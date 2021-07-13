// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace System.ComponentModel.Composition
{
    // We need a public version of CompositionErrorId, so that the QA tests can access and verify the errors.
    public enum ErrorId : int
    {
        Unknown = CompositionErrorId.Unknown,
        InvalidExportMetadata = CompositionErrorId.InvalidExportMetadata,
        RequiredMetadataNotFound = CompositionErrorId.RequiredMetadataNotFound,
        UnsupportedExportType = CompositionErrorId.UnsupportedExportType,
        ImportNotSetOnPart = CompositionErrorId.ImportNotSetOnPart,
        CompositionEngine_ComposeTookTooManyIterations = CompositionErrorId.CompositionEngine_ComposeTookTooManyIterations,
        CompositionEngine_ImportCardinalityMismatch = CompositionErrorId.CompositionEngine_ImportCardinalityMismatch,        
        CompositionEngine_PartCycle = CompositionErrorId.CompositionEngine_PartCycle,
        CompositionEngine_PartCannotSetImport = CompositionErrorId.CompositionEngine_PartCannotSetImport,
        CompositionEngine_PartCannotGetExportedObject = CompositionErrorId.CompositionEngine_PartCannotGetExportedObject,
        CompositionEngine_PartCannotActivate = CompositionErrorId.CompositionEngine_PartCannotActivate,
        ReflectionModel_PartConstructorMissing = CompositionErrorId.ReflectionModel_PartConstructorMissing,
        ReflectionModel_PartConstructorThrewException = CompositionErrorId.ReflectionModel_PartConstructorThrewException,
        ReflectionModel_PartOnImportsSatisfiedThrewException = CompositionErrorId.ReflectionModel_PartOnImportsSatisfiedThrewException,
        ReflectionModel_ExportNotReadable = CompositionErrorId.ReflectionModel_ExportNotReadable,
        ReflectionModel_ExportThrewException = CompositionErrorId.ReflectionModel_ExportThrewException,
        ReflectionModel_ExportMethodTooManyParameters = CompositionErrorId.ReflectionModel_ExportMethodTooManyParameters,
        ReflectionModel_ImportNotWritable = CompositionErrorId.ReflectionModel_ImportNotWritable,
        ReflectionModel_ImportThrewException = CompositionErrorId.ReflectionModel_ImportThrewException,
        ReflectionModel_ImportNotAssignableFromExport = CompositionErrorId.ReflectionModel_ImportNotAssignableFromExport,
        ReflectionModel_ImportCollectionNull = CompositionErrorId.ReflectionModel_ImportCollectionNull,
        ReflectionModel_ImportCollectionNotWritable = CompositionErrorId.ReflectionModel_ImportCollectionNotWritable,
        ReflectionModel_ImportCollectionConstructionThrewException = CompositionErrorId.ReflectionModel_ImportCollectionConstructionThrewException,
        ReflectionModel_ImportCollectionGetThrewException = CompositionErrorId.ReflectionModel_ImportCollectionGetThrewException,
        ReflectionModel_ImportCollectionIsReadOnlyThrewException = CompositionErrorId.ReflectionModel_ImportCollectionIsReadOnlyThrewException,
        ReflectionModel_ImportCollectionClearThrewException = CompositionErrorId.ReflectionModel_ImportCollectionClearThrewException,
        ReflectionModel_ImportCollectionAddThrewException = CompositionErrorId.ReflectionModel_ImportCollectionAddThrewException,
        Adapter_CannotAdaptNullOrEmptyFromOrToContract = CompositionErrorId.Adapter_CannotAdaptNullOrEmptyFromOrToContract,
        Adapter_CannotAdaptFromAndToSameContract = CompositionErrorId.Adapter_CannotAdaptFromAndToSameContract,
        Adapter_ContractMismatch = CompositionErrorId.Adapter_ContractMismatch,
        Adapter_TypeMismatch = CompositionErrorId.Adapter_TypeMismatch,
        Adapter_ExceptionDuringAdapt = CompositionErrorId.Adapter_ExceptionDuringAdapt,
    }
}
