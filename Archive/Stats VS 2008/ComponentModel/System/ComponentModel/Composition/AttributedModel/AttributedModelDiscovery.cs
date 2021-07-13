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
using Microsoft.Internal;

namespace System.ComponentModel.Composition.AttributedModel
{
    internal static class AttributedModelDiscovery
    {
        public static ComposablePartDefinition CreatePartDefinitionIfDiscoverable(Type type, ICompositionElement origin)
        {
            if (type.IsAttributeDefined<PartNotDiscoverableAttribute>())
            {
                return null;
            }

            AttributedPartCreationInfo creationInfo = new AttributedPartCreationInfo(type, null, false, origin);

            if (!creationInfo.IsPartDiscoverable())
            {
                return null;
            }

            return new ReflectionComposablePartDefinition(creationInfo);
        }

        public static ReflectionComposablePartDefinition CreatePartDefinition(Type type, PartCreationPolicyAttribute partCreationPolicy, bool ignoreConstructorImports, ICompositionElement origin)
        {
            Assumes.NotNull(type);

            AttributedPartCreationInfo creationInfo = new AttributedPartCreationInfo(type, partCreationPolicy, ignoreConstructorImports, origin);

            return new ReflectionComposablePartDefinition(creationInfo);
        }

        public static ReflectionComposablePart CreatePart(object attributedPart)
        {
            Assumes.NotNull(attributedPart);

            // If given an instance then we want to pass the default composition options because we treat it as a shared part
            // TODO: ICompositionElement Give this def an origin indicating that it was added directly to the MutableExportProvider.

            ReflectionComposablePartDefinition definition = AttributedModelDiscovery.CreatePartDefinition(attributedPart.GetType(), PartCreationPolicyAttribute.Shared, true, (ICompositionElement)null);

            return new ReflectionComposablePart(definition, attributedPart);
        }

        public static ReflectionParameterImportDefinition CreateParameterImportDefinition(ParameterInfo parameter, ICompositionElement origin)
        {
            Requires.NotNull(parameter, "parameter");

            ReflectionParameter reflectionParameter = parameter.ToReflectionParameter();

            AttributedImportDefinitionCreationInfo importCreationInfo = AttributedModelDiscovery.GetImportDefinitionCreationInfo(reflectionParameter, parameter);
            return new ReflectionParameterImportDefinition(
                parameter.AsLazy(), 
                importCreationInfo.ContractName,
                importCreationInfo.RequiredTypeIdentity,
                importCreationInfo.RequiredMetadata, 
                importCreationInfo.Cardinality, 
                importCreationInfo.RequiredCreationPolicy,
                origin);
        }

        public static ReflectionMemberImportDefinition CreateMemberImportDefinition(MemberInfo member, ICompositionElement origin)
        {
            Requires.NotNull(member, "member");

            ReflectionWritableMember reflectionMember = member.ToReflectionWritableMember();

            AttributedImportDefinitionCreationInfo importCreationInfo = AttributedModelDiscovery.GetImportDefinitionCreationInfo(reflectionMember, member);
            return new ReflectionMemberImportDefinition(
                new LazyMemberInfo(member), 
                importCreationInfo.ContractName, 
                importCreationInfo.RequiredTypeIdentity,
                importCreationInfo.RequiredMetadata, 
                importCreationInfo.Cardinality, 
                importCreationInfo.IsRecomposable, 
                importCreationInfo.RequiredCreationPolicy,
                origin);
        }

        //
        // Import definition creation helpers
        //
        private static AttributedImportDefinitionCreationInfo GetImportDefinitionCreationInfo(ReflectionItem item, ICustomAttributeProvider attributeProvider)
        {
            Assumes.NotNull(item, attributeProvider);

            AttributedImportDefinitionCreationInfo importCreationInfo = new AttributedImportDefinitionCreationInfo();

            IAttributedImport attributedImport = AttributedModelDiscovery.GetAttributedImport(item, attributeProvider);
            ImportType importType = new ImportType(item.ReturnType);

            importCreationInfo.RequiredMetadata = importType.IsLazy ?
                    CompositionServices.GetRequiredMetadata(importType.LazyType.MetadataViewType) :
                    Enumerable.Empty<string>();
            importCreationInfo.Cardinality = AttributedModelDiscovery.GetCardinality(item, attributedImport, importType);
            importCreationInfo.ContractName = attributedImport.GetContractNameFromImport(importCreationInfo.Cardinality, importType);
            importCreationInfo.RequiredTypeIdentity = attributedImport.GetTypeIdentityFromImport(importCreationInfo.Cardinality, importType);
            importCreationInfo.IsRecomposable = (item.ItemType == ReflectionItemType.Parameter) ? false : attributedImport.AllowRecomposition;
            importCreationInfo.RequiredCreationPolicy = attributedImport.RequiredCreationPolicy;

            return importCreationInfo;
        }

        private static IAttributedImport GetAttributedImport(ReflectionItem item, ICustomAttributeProvider attributeProvider)
        {
            IAttributedImport[] imports = attributeProvider.GetAttributes<IAttributedImport>(false);

            // For constructor parameters they may not have an ImportAttribute
            if (imports.Length == 0)
            {
                return new ImportAttribute();
            }

            if (imports.Length == 1)
            {
                return imports[0];
            }

            // DiscoveryError (Dev10:602872): This should go through the discovery error reporting when 
            // we add a way to report discovery errors properly.
            throw ExceptionBuilder.CreateDiscoveryException(Strings.Discovery_MultipleImportAttributes, item.GetDisplayName());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "item")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "importType")]
        private static ImportCardinality GetCardinality(ReflectionItem item, IAttributedImport attributedImport, ImportType importType)
        {
            ImportCardinality cardinality = attributedImport.Cardinality;

#if !SILVERLIGHT
            // Dev10:602887 In order to help mitigate the breaking change of adding ImportMany we are going
            // to support the old behavior of automatically trying to detect if a type is a should
            // be treated as a collection or not but it will be removed at some later point.
            // Ultimately people should move over to ImportMany.
            if (cardinality != ImportCardinality.ZeroOrMore)
            {
                if (importType.ElementType != null)
                {
                    System.Diagnostics.Debug.WriteLine("Use ImportMany on " + item.GetDisplayName());

                    if (Environment.GetEnvironmentVariable("ONLY_ALLOW_IMPORTMANY") == null)
                    {
                        cardinality = ImportCardinality.ZeroOrMore;
                    }
                }
            }
#endif

            return cardinality;
        }

        private struct AttributedImportDefinitionCreationInfo
        {
            public string ContractName { get; set; }
            public string RequiredTypeIdentity { get; set; }
            public IEnumerable<string> RequiredMetadata { get; set; }
            public ImportCardinality Cardinality { get; set; }
            public bool IsRecomposable { get; set; }
            public CreationPolicy RequiredCreationPolicy { get; set; }
        }
    }
}
