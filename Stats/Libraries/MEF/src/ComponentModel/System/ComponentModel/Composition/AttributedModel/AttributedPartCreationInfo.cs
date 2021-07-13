// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition.AttributedModel
{
    internal class AttributedPartCreationInfo : IReflectionPartCreationInfo
    {
        private readonly Type _type;
        private readonly bool _ignoreConstructorImports = false;
        private readonly ICompositionElement _origin;
        private PartCreationPolicyAttribute _partCreationPolicy = null;
        private ConstructorInfo _constructor;
        private IEnumerable<ExportDefinition> _exports;
        private IEnumerable<ImportDefinition> _imports;

        public AttributedPartCreationInfo(Type type, PartCreationPolicyAttribute partCreationPolicy, bool ignoreConstructorImports, ICompositionElement origin)
        {
            Assumes.NotNull(type);
            this._type = type;
            this._ignoreConstructorImports = ignoreConstructorImports;
            this._partCreationPolicy = partCreationPolicy;
            this._origin = origin;
        }

        public Type GetPartType()
        {
            return this._type;
        }

        public LazyInit<Type> GetLazyPartType()
        {
            return new LazyInit<Type>(this.GetPartType);
        }

        public ConstructorInfo GetConstructor()
        {
            if (this._constructor == null && !this._ignoreConstructorImports)
            {
                this._constructor = SelectPartConstructor(this._type);
            }
            return this._constructor;
        }

        public IDictionary<string, object> GetMetadata()
        {
            return this._type.GetPartMetadataForType(this.CreationPolicy);
        }

        public IEnumerable<ExportDefinition> GetExports()
        {
            DiscoverExportsAndImports();
            return this._exports;
        }

        public IEnumerable<ImportDefinition> GetImports()
        {
            DiscoverExportsAndImports();
            return this._imports;
        }

        public bool IsPartDiscoverable()
        {
            if (this._type.ContainsGenericParameters)
            {
                return false;
            }

            return GetExportMembers(this._type).Any();
        }

        string ICompositionElement.DisplayName
        {
            get { return this.GetDisplayName(); }
        }

        ICompositionElement ICompositionElement.Origin
        {
            get { return this._origin; }
        }

        public override string ToString()
        {
            return GetDisplayName();
        }

        private string GetDisplayName()
        {
            return this.GetPartType().GetDisplayName();
        }

        private CreationPolicy CreationPolicy
        {
            get
            {
                if (this._partCreationPolicy == null)
                {
                    this._partCreationPolicy = this._type.GetFirstAttribute<PartCreationPolicyAttribute>() ?? PartCreationPolicyAttribute.Default;
                }
                return this._partCreationPolicy.CreationPolicy;
            }
        }

        private static ConstructorInfo SelectPartConstructor(Type type)
        {
            Assumes.NotNull(type);

            if (type.IsAbstract)
            {
                return null;
            }

            // Only deal with non-static constructors
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            ConstructorInfo[] constructors = type.GetConstructors(flags);

            // Should likely only happen for static or abstract types
            if (constructors.Length == 0)
            {
                return null;
            }

            // Optimize single default constructor.
            if (constructors.Length == 1 && constructors[0].GetParameters().Length == 0)
            {
                return constructors[0];
            }

            // Select the marked constructor if there is exactly one marked
            IEnumerable<ConstructorInfo> importingConstructors = constructors.Where(
                ctor => ctor.IsAttributeDefined<ImportingConstructorAttribute>());

            switch (importingConstructors.GetCardinality())
            {
                case EnumerableCardinality.One:
                    {
                        return importingConstructors.First();
                    }

                case EnumerableCardinality.TwoOrMore:
                    {
                        // Return null, the part will error on instantiation.
                        return null;
                    }
            }

            // If there are no marked constructors then select the default constructor
            IEnumerable<ConstructorInfo> defaultConstructors = constructors.Where(
                ctor => ctor.GetParameters().Length == 0);

            // There should only ever be zero or one default constructors  
            return defaultConstructors.SingleOrDefault();
        }

        private void DiscoverExportsAndImports()
        {
            if (this._exports != null)
            {
                return;
            }

            this._exports = GetExportDefinitions();
            this._imports = GetImportDefinitions();
        }

        private IEnumerable<ExportDefinition> GetExportDefinitions()
        {
            List<ExportDefinition> exports = new List<ExportDefinition>();

            foreach (MemberInfo member in GetExportMembers(this._type))
            {
                foreach (ExportAttribute exportAttribute in member.GetAttributes<ExportAttribute>())
                {
                    var attributedExportDefinition = new AttributedExportDefinition(this, member, exportAttribute);
                    exports.Add(new ReflectionMemberExportDefinition(member.ToLazyMember(), attributedExportDefinition, this));
                }
            }

            return exports;
        }

        private IEnumerable<MemberInfo> GetExportMembers(Type type)
        {
            BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public |
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            bool staticOnly = type.IsAbstract;

            // If the type is abstract only find local static exports
            if (staticOnly)
            {
                flags &= ~BindingFlags.Instance;
            }

            foreach (MemberInfo member in GetDeclaredOnlyExportMembers(type, flags))
            {
                yield return member;
            }

            if (!staticOnly)
            {
                foreach (Type iface in type.GetInterfaces())
                {
                    if (iface.IsAttributeDefined<PartExportsInheritedAttribute>())
                    {
                        foreach (MemberInfo member in GetDeclaredOnlyExportMembers(iface, flags))
                        {
                            yield return member;
                        }
                    }
                }

                // Walk up the type chain until you hit object.
                if (type.BaseType != null)
                {
                    // Only interested in instance on base types.
                    flags &= ~BindingFlags.Static;

                    Type baseType = type.BaseType;

                    // Stopping at object instead of null to help with performance. It is a noticable performance
                    // gain (~5%) if we don't have to try and pull the attributes we know don't exist on object.
                    while (baseType != typeof(object))
                    {
                        if (baseType.IsAttributeDefined<PartExportsInheritedAttribute>())
                        {
                            foreach (MemberInfo member in GetDeclaredOnlyExportMembers(baseType, flags))
                            {
                                yield return member;
                            }
                        }
                        baseType = baseType.BaseType;
                    }
                }
            }
        }

        private IEnumerable<MemberInfo> GetDeclaredOnlyExportMembers(Type type, BindingFlags flags)
        {
            if ((flags & BindingFlags.Instance) == BindingFlags.Instance
                && IsExport(type))
            {
                yield return type;
            }

            // Walk the fields 
            foreach (var member in type.GetFields(flags))
            {
                if (IsExport(member))
                {
                    yield return member;
                }
            }

            // Walk the properties 
            foreach (var member in type.GetProperties(flags))
            {
                if (IsExport(member))
                {
                    yield return member;
                }
            }

            // Walk the methods 
            foreach (var member in type.GetMethods(flags))
            {
                if (IsExport(member))
                {
                    yield return member;
                }
            }
        }

        private static bool IsExport(ICustomAttributeProvider attributeProvider)
        {
            return attributeProvider.IsAttributeDefined<ExportAttribute>(false);
        }

        private IEnumerable<ImportDefinition> GetImportDefinitions()
        {
            List<ImportDefinition> imports = new List<ImportDefinition>();

            foreach (MemberInfo member in GetImportMembers(this._type))
            {
                ReflectionMemberImportDefinition importDefinition = AttributedModelDiscovery.CreateMemberImportDefinition(member, this);
                imports.Add(importDefinition);
            }

            var constructor = this.GetConstructor();

            if (constructor != null)
            {
                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    ReflectionParameterImportDefinition importDefinition = AttributedModelDiscovery.CreateParameterImportDefinition(parameter, this);
                    imports.Add(importDefinition);
                }
            }

            return imports;
        }

        private IEnumerable<MemberInfo> GetImportMembers(Type type)
        {
            if (type.IsAbstract)
            {
                yield break;
            }

            foreach (MemberInfo member in GetDeclaredOnlyImportMembers(type))
            {
                yield return member;
            }

            foreach (Type iface in type.GetInterfaces())
            {
                foreach (MemberInfo member in GetDeclaredOnlyImportMembers(iface))
                {
                    yield return member;
                }
            }

            // Walk up the type chain until you hit object.
            if (type.BaseType != null)
            {
                Type baseType = type.BaseType;

                // Stopping at object instead of null to help with performance. It is a noticable performance
                // gain (~5%) if we don't have to try and pull the attributes we know don't exist on object.
                while (baseType != typeof(object))
                {
                    foreach (MemberInfo member in GetDeclaredOnlyImportMembers(baseType))
                    {
                        yield return member;
                    }
                    baseType = baseType.BaseType;
                }
            }
        }

        private IEnumerable<MemberInfo> GetDeclaredOnlyImportMembers(Type type)
        {
            BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            // Walk the fields 
            foreach (var member in type.GetFields(flags))
            {
                if (IsImport(member))
                {
                    yield return member;
                }
            }

            // Walk the properties 
            foreach (var member in type.GetProperties(flags))
            {
                if (IsImport(member))
                {
                    yield return member;
                }
            }
        }

        private static bool IsImport(ICustomAttributeProvider attributeProvider)
        {
            return attributeProvider.IsAttributeDefined<IAttributedImport>(false);
        }
    }
}
