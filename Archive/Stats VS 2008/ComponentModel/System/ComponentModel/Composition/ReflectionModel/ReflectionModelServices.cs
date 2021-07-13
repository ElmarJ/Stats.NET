// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;
using Microsoft.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Globalization;

namespace System.ComponentModel.Composition.ReflectionModel
{
    public static class ReflectionModelServices
    {
        public static LazyInit<Type> GetPartType(ComposablePartDefinition partDefinition)
        {
            Requires.NotNull(partDefinition, "partDefinition");

            ReflectionComposablePartDefinition reflectionPartDefinition = partDefinition as ReflectionComposablePartDefinition;
            if (reflectionPartDefinition == null)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_InvalidPartDefinition, partDefinition.GetType()),
                    "partDefinition");
            }

            return reflectionPartDefinition.GetLazyPartType();
        }

        public static LazyMemberInfo GetExportingMember(ExportDefinition exportDefinition)
        {
            Requires.NotNull(exportDefinition, "exportDefinition");

            ReflectionMemberExportDefinition reflectionExportDefinition = exportDefinition as ReflectionMemberExportDefinition;
            if (reflectionExportDefinition == null)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_InvalidExportDefinition, exportDefinition.GetType()),
                    "exportDefinition");
            }

            return reflectionExportDefinition.ExportingLazyMember;
        }

        public static LazyMemberInfo GetImportingMember(ImportDefinition importDefinition)
        {
            Requires.NotNull(importDefinition, "importDefinition");

            ReflectionMemberImportDefinition reflectionMemberImportDefinition = importDefinition as ReflectionMemberImportDefinition;
            if (reflectionMemberImportDefinition == null)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_InvalidMemberImportDefinition, importDefinition.GetType()),
                    "importDefinition");
            }

            return reflectionMemberImportDefinition.ImportingLazyMember;
        }

        public static LazyInit<ParameterInfo> GetImportingParameter(ImportDefinition importDefinition)
        {
            ReflectionParameterImportDefinition reflectionParameterImportDefinition = importDefinition as ReflectionParameterImportDefinition;
            if (reflectionParameterImportDefinition == null)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_InvalidParameterImportDefinition, importDefinition.GetType()),
                    "importDefinition");
            }

            return reflectionParameterImportDefinition.ImportingLazyParameter;
        }

        public static bool IsImportingParameter(ImportDefinition importDefinition)
        {
            ReflectionImportDefinition reflectionImportDefinition = importDefinition as ReflectionImportDefinition;
            if (reflectionImportDefinition == null)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Strings.ReflectionModel_InvalidImportDefinition, importDefinition.GetType()),
                    "importDefinition");
            }

            return (importDefinition is ReflectionParameterImportDefinition);
        }

        public static ComposablePartDefinition CreatePartDefinition(
            LazyInit<Type> partType,
            LazyInit<IEnumerable<ImportDefinition>> imports,
            LazyInit<IEnumerable<ExportDefinition>> exports,
            LazyInit<IDictionary<string, object>> metadata,
            ICompositionElement origin)
        {
            Requires.NotNull(partType, "partType");
            Requires.NotNull(imports, "imports");
            Requires.NotNull(exports, "exports");

            return new ReflectionComposablePartDefinition(
                new ReflectionPartCreationInfo(
                    partType,
                    imports,
                    exports,
                    metadata,
                    origin));
        }

        public static ExportDefinition CreateExportDefinition(
            LazyMemberInfo exportingMember,
            string contractName,
            LazyInit<IDictionary<string, object>> metadata,
            ICompositionElement origin)
        {
            Requires.NotNullOrEmpty(contractName, "contractName");

            return new ReflectionMemberExportDefinition(
                exportingMember,
                new LazyExportDefinition(contractName, metadata),
                origin);
        }
        
        public static ContractBasedImportDefinition CreateImportDefinition(
            LazyMemberInfo importingMember,
            string contractName,
            string requiredTypeIdentity,
            IEnumerable<string> requiredMetadata,
            ImportCardinality cardinality,
            bool isRecomposable,
            CreationPolicy requiredCreationPolicy,
            ICompositionElement origin)
        {
            Requires.NotNullOrEmpty(contractName, "contractName");
            Requires.NotNullOrNullElements(requiredMetadata, "requiredMetadata");
            Requires.IsInMembertypeSet(importingMember.MemberType, "importingMember", MemberTypes.Property | MemberTypes.Field);

            return new ReflectionMemberImportDefinition(
                importingMember,
                contractName,
                requiredTypeIdentity,
                requiredMetadata,
                cardinality,
                isRecomposable,
                requiredCreationPolicy,
                origin);
        }

        public static ContractBasedImportDefinition CreateImportDefinition(
            LazyInit<ParameterInfo> parameter,
            string contractName,
            string requiredTypeIdentity,
            IEnumerable<string> requiredMetadata,
            ImportCardinality cardinality,
            CreationPolicy requiredCreationPolicy,
            ICompositionElement origin)
        {
            Requires.NotNull(parameter, "parameter");
            Requires.NotNullOrEmpty(contractName, "contractName");
            Requires.NotNullOrNullElements(requiredMetadata, "requiredMetadata");

            return new ReflectionParameterImportDefinition(
                parameter,
                contractName,
                requiredTypeIdentity,
                requiredMetadata,
                cardinality,
                requiredCreationPolicy,
                origin);
        }

        private class ReflectionPartCreationInfo : IReflectionPartCreationInfo
        {
            private readonly LazyInit<Type> _partType;
            private readonly LazyInit<IEnumerable<ImportDefinition>> _imports;
            private readonly LazyInit<IEnumerable<ExportDefinition>> _exports;
            private readonly LazyInit<IDictionary<string, object>> _metadata;
            private readonly ICompositionElement _origin;
            private ConstructorInfo _constructor;

            public ReflectionPartCreationInfo(
                LazyInit<Type> partType,
                LazyInit<IEnumerable<ImportDefinition>> imports,
                LazyInit<IEnumerable<ExportDefinition>> exports,
                LazyInit<IDictionary<string, object>> metadata,
                ICompositionElement origin)
            {
                this._partType = partType;
                this._imports = imports;
                this._exports = exports;
                this._metadata = metadata;
                this._origin = origin;
            }

            public Type GetPartType()
            {
                return this._partType.Value;
            }

            public LazyInit<Type> GetLazyPartType()
            {
                return this._partType;
            }

            public ConstructorInfo GetConstructor()
            {
                if (this._constructor == null)
                {
                    ConstructorInfo[] constructors = null;
                    constructors = this.GetImports()
                        .OfType<ReflectionParameterImportDefinition>()
                        .Select(parameterImport => parameterImport.ImportingLazyParameter.Value.Member)
                        .OfType<ConstructorInfo>()
                        .Distinct()
                        .ToArray();

                    if (constructors.Length == 1)
                    {
                        this._constructor = constructors[0];
                    }
                    else if (constructors.Length == 0)
                    {
                        this._constructor = this.GetPartType().GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                    }
                }
                return this._constructor;
            }

            public IDictionary<string, object> GetMetadata()
            {
                return this._metadata.Value;
            }

            public IEnumerable<ExportDefinition> GetExports()
            {
                return this._exports.Value;
            }

            public IEnumerable<ImportDefinition> GetImports()
            {
                return this._imports.Value;
            }

            public string DisplayName
            {
                get { return this.GetPartType().GetDisplayName(); }
            }

            public ICompositionElement Origin
            {
                get { return this._origin; }
            }
        }

        private class LazyExportDefinition : ExportDefinition
        {
            private readonly LazyInit<IDictionary<string, object>> _metadata;

            public LazyExportDefinition(string contractName, LazyInit<IDictionary<string, object>> metadata)
                : base(contractName, (IDictionary<string, object>)null)
            {
                this._metadata = metadata;
            }

            public override IDictionary<string, object> Metadata
            {
                get
                {
                    return this._metadata.Value;
                }
            }
        }

    }
}
