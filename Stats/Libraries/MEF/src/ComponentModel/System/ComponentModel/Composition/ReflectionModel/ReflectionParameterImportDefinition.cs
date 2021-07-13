﻿// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Internal;
using System.ComponentModel.Composition.ReflectionModel;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Threading;

namespace System.ComponentModel.Composition.ReflectionModel
{
    internal class ReflectionParameterImportDefinition : ReflectionImportDefinition
    {
        private LazyInit<ParameterInfo> _importingLazyParameter;

        public ReflectionParameterImportDefinition(
            LazyInit<ParameterInfo> importingLazyParameter,
            string contractName, 
            string requiredTypeIdentity,
            IEnumerable<string> requiredMetadata,
            ImportCardinality cardinality, 
            CreationPolicy requiredCreationPolicy,
            ICompositionElement origin) 
            : base(contractName, requiredTypeIdentity, requiredMetadata, cardinality, false, true, requiredCreationPolicy, origin)
        {
            Assumes.NotNull(importingLazyParameter);

            this._importingLazyParameter = importingLazyParameter;
        }

        public override ImportingItem ToImportingItem()
        {
            return new ImportingParameter(this, new ImportType(this.ImportingLazyParameter.GetNotNullValue("parameter").ParameterType));
        }

        public LazyInit<ParameterInfo> ImportingLazyParameter
        {
            get { return this._importingLazyParameter; }
        }

        protected override string GetDisplayName()
        {
            ParameterInfo parameter = this.ImportingLazyParameter.GetNotNullValue("parameter");
            return string.Format(
                CultureInfo.CurrentCulture,
                "{0} (Parameter=\"{1}\", ContractName=\"{2}\")",  // NOLOC
                parameter.Member.GetDisplayName(),
                parameter.Name,                
                this.ContractName);
        }
    }
}
