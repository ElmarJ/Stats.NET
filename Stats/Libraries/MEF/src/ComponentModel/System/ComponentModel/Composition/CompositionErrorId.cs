// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace System.ComponentModel.Composition
{
    internal enum CompositionErrorId : int
    {
        Unknown = 0,
        InvalidExportMetadata,
        RequiredMetadataNotFound,
        UnsupportedExportType,
        ImportNotSetOnPart,
        CompositionEngine_ComposeTookTooManyIterations,        
        CompositionEngine_ImportCardinalityMismatch,
        CompositionEngine_PartCycle,
        CompositionEngine_PartCannotSetImport,
        CompositionEngine_PartCannotGetExportedObject,
        CompositionEngine_PartCannotActivate,
        ReflectionModel_PartConstructorMissing,
        ReflectionModel_PartConstructorThrewException,
        ReflectionModel_PartOnImportsSatisfiedThrewException,
        ReflectionModel_ExportNotReadable,
        ReflectionModel_ExportThrewException,
        ReflectionModel_ExportMethodTooManyParameters,
        ReflectionModel_ImportNotWritable,
        ReflectionModel_ImportThrewException,
        ReflectionModel_ImportNotAssignableFromExport,        
        ReflectionModel_ImportCollectionNull,
        ReflectionModel_ImportCollectionNotWritable,
        ReflectionModel_ImportCollectionConstructionThrewException,
        ReflectionModel_ImportCollectionGetThrewException,
        ReflectionModel_ImportCollectionIsReadOnlyThrewException,
        ReflectionModel_ImportCollectionClearThrewException,
        ReflectionModel_ImportCollectionAddThrewException,
        Adapter_CannotAdaptNullOrEmptyFromOrToContract,
        Adapter_CannotAdaptFromAndToSameContract,
        Adapter_ContractMismatch,
        Adapter_TypeMismatch,
        Adapter_ExceptionDuringAdapt,
    }
}
