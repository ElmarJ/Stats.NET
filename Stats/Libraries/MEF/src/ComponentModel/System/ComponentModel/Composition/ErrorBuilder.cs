// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Internal;

namespace System.ComponentModel.Composition
{
    internal static class ErrorBuilder
    {
        public static CompositionError ComposeTookTooManyIterations(int maximumNumberOfCompositionIterations)
        {
            return CompositionError.Create(
                CompositionErrorId.CompositionEngine_ComposeTookTooManyIterations,
                Strings.CompositionEngine_ComposeTookTooManyIterations,
                maximumNumberOfCompositionIterations);
        }

        public static CompositionError CreateImportCardinalityMismatch(ImportCardinalityMismatchException exception, ImportDefinition definition)
        {
            Assumes.NotNull(exception, definition);

            return CompositionError.Create(
                CompositionErrorId.CompositionEngine_ImportCardinalityMismatch, 
                exception.Message,
                definition.ToElement(), 
                (Exception)null);
        }

        public static CompositionError CreatePartCannotActivate(ComposablePart part, Exception innerException)
        {
            Assumes.NotNull(part, innerException);

            ICompositionElement element = part.ToElement();
            return CompositionError.Create(
                CompositionErrorId.CompositionEngine_PartCannotActivate,
                element,
                innerException,
                Strings.CompositionEngine_PartCannotActivate,
                element.DisplayName);
        }

        public static CompositionError CreatePartCannotSetImport(ComposablePart part, ImportDefinition definition, Exception innerException)
        {
            Assumes.NotNull(part, definition, innerException);

            ICompositionElement element = definition.ToElement();
            return CompositionError.Create(
                CompositionErrorId.CompositionEngine_PartCannotSetImport,
                element,
                innerException,
                Strings.CompositionEngine_PartCannotSetImport,
                element.DisplayName,
                part.ToElement().DisplayName);
        }

        public static CompositionError CreateCannotGetExportedObject(ComposablePart part, ExportDefinition definition, Exception innerException)
        {
            Assumes.NotNull(part, definition, innerException);

            ICompositionElement element = definition.ToElement();
            return CompositionError.Create(
                CompositionErrorId.CompositionEngine_PartCannotGetExportedObject,
                element,
                innerException,
                Strings.CompositionEngine_PartCannotGetExportedObject,
                element.DisplayName,
                part.ToElement().DisplayName);
        }

        public static CompositionError CreatePartCycle(ComposablePart part)
        {
            Assumes.NotNull(part);

            ICompositionElement element = part.ToElement();
            return CompositionError.Create(
                CompositionErrorId.CompositionEngine_PartCycle,
                element,
                Strings.CompositionEngine_PartCycle,
                element.DisplayName);
        }
    }
}
