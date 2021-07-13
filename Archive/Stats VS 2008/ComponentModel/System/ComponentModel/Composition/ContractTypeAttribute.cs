// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.ComponentModel.Composition.Hosting;

namespace System.ComponentModel.Composition
{
    /// <summary>
    ///     Specifies that a class, interface or delegate is a contract type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
    public sealed class ContractTypeAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ContractTypeAttribute"/> class, using the 
        ///     default contract name.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on the type that is 
        ///         marked with this attribute.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        public ContractTypeAttribute()
            : this((string)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportAttribute"/> class with the 
        ///     specified contract name.
        /// </summary>
        /// <param name="contractName">
        ///      A <see cref="String"/> containing the contract name of the type, or 
        ///      <see langword="null"/> or an empty string ("") to use the default contract name.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         The default contract name is the result of calling 
        ///         <see cref="AttributedModelServices.GetContractName(Type)"/> on the type that is 
        ///         marked with this attribute.
        ///     </para>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison 
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        public ContractTypeAttribute(string contractName)
        {
            this.ContractName = contractName ?? string.Empty;
        }

        /// <summary>
        ///     Gets the contract name of the contract type.
        /// </summary>
        /// <value>
        ///      A <see cref="String"/> containing the contract name of the contract 
        ///      <see cref="Type"/>. The default value is an empty string ("").
        /// </value>
        public string ContractName { get; private set; }

        /// <summary>
        ///     Gets the metadata view of the contract type.
        /// </summary>
        /// <value>
        ///     A <see cref="Type"/> representing the metadata view of the contract 
        ///     <see cref="Type"/>. The default value is <see langword="null"/>.
        /// </value>
        public Type MetadataViewType { get; set; }
    }
}
