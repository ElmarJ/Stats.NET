// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Internal.Collections;

namespace System.ComponentModel.Composition
{
    internal struct CompositionResult
    {
        public static readonly CompositionResult SucceededResult = new CompositionResult();
        private readonly IEnumerable<CompositionError> _errors;

        public CompositionResult(params CompositionError[] errors)
            : this((IEnumerable<CompositionError>)errors)
        {            
        }

        public CompositionResult(IEnumerable<CompositionError> errors)
        {
            _errors = errors;
        }

        public bool Succeeded
        {
            get { return this._errors == null || !this._errors.FastAny(); }
        }

        public IEnumerable<CompositionError> Errors
        {
            get { return this._errors ?? Enumerable.Empty<CompositionError>(); }
        }

        public CompositionResult MergeResult(CompositionResult result)
        {
            return MergeErrors(result._errors);
        }

        public CompositionResult MergeError(CompositionError error)
        {
            return MergeErrors(new CompositionError[] { error });
        }

        public CompositionResult MergeErrors(IEnumerable<CompositionError> errors)
        {
            return new CompositionResult(_errors.ConcatAllowingNull(errors));
        }

        public CompositionResult<T> ToResult<T>(T value)
        {
            return new CompositionResult<T>(value, this._errors); 
        }

        public void ThrowOnErrors()
        {
            if (!this.Succeeded)
            {
                throw new CompositionException(this._errors);
            }
        }
    }
}
