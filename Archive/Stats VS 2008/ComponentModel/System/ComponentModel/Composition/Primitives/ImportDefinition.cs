// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Primitives
{
    /// <summary>
    ///     Represents an import required by a <see cref="ComposablePart"/> object.
    /// </summary>
    public class ImportDefinition
    {
        private readonly Expression<Func<ExportDefinition, bool>> _constraint;
        private readonly ImportCardinality _cardinality = ImportCardinality.ExactlyOne;
        private readonly bool _isRecomposable;
        private readonly bool _isPrerequisite = true;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportDefinition"/> class.
        /// </summary>
        /// <remarks>
        ///     <note type="inheritinfo">
        ///         Derived types calling this constructor must override the <see cref="Constraint"/> 
        ///         property, and optionally, the <see cref="Cardinality"/>, <see cref="IsPrerequisite"/> 
        ///         and <see cref="IsRecomposable"/> 
        ///         properties.
        ///     </note>
        /// </remarks>
        protected ImportDefinition()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportDefinition"/> class 
        ///     with the specified constraint, cardinality, value indicating if the import 
        ///     definition is recomposable and a value indicating if the import definition 
        ///     is a prerequisite.
        /// </summary>
        /// <param name="constraint">
        ///     A <see cref="Expression{TDelegate}"/> containing a <see cref="Func{T, TResult}"/> 
        ///     that defines the conditions that must be matched for the <see cref="ImportDefinition"/> 
        ///     to be satisfied by an <see cref="Export"/>.
        /// </param>
        /// <param name="cardinality">
        ///     One of the <see cref="ImportCardinality"/> values indicating the 
        ///     cardinality of the <see cref="Export"/> objects required by the
        ///     <see cref="ImportDefinition"/>.
        /// </param>
        /// <param name="isRecomposable">
        ///     <see langword="true"/> if the <see cref="ImportDefinition"/> can be satisfied 
        ///     multiple times throughout the lifetime of a <see cref="ComposablePart"/>, otherwise, 
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="isPrerequisite">
        ///     <see langword="true"/> if the <see cref="ImportDefinition"/> is required to be 
        ///     satisfied before a <see cref="ComposablePart"/> can start producing exported 
        ///     objects; otherwise, <see langword="false"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constraint"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="cardinality"/> is not one of the <see cref="ImportCardinality"/> 
        ///     values.
        /// </exception>
        public ImportDefinition(Expression<Func<ExportDefinition, bool>> constraint, ImportCardinality cardinality, bool isRecomposable, bool isPrerequisite)
            : this(cardinality, isRecomposable, isPrerequisite)
        {
            Requires.NotNull(constraint, "constraint");

            this._constraint = constraint;
        }

        internal ImportDefinition(ImportCardinality cardinality, bool isRecomposable, bool isPrerequisite)
        {
            Requires.IsDefined<ImportCardinality>(cardinality, "cardinality");

            this._cardinality = cardinality;
            this._isRecomposable = isRecomposable;
            this._isPrerequisite = isPrerequisite;
        }

        /// <summary>
        ///     Gets the cardinality of the exports required by the import definition.
        /// </summary>
        /// <value>
        ///     One of the <see cref="ImportCardinality"/> values indicating the 
        ///     cardinality of the <see cref="Export"/> objects required by the
        ///     <see cref="ImportDefinition"/>. The default is 
        ///     <see cref="ImportCardinality.ExactlyOne"/>
        /// </value>
        public virtual ImportCardinality Cardinality
        {
            get { return _cardinality; }
        }

        /// <summary>
        ///     Gets an expression that defines conditions that must be matched for the import 
        ///     described by the import definition to be satisfied.
        /// </summary>
        /// <returns>
        ///     A <see cref="Expression{TDelegate}"/> containing a <see cref="Func{T, TResult}"/> 
        ///     that defines the conditions that must be matched for the 
        ///     <see cref="ImportDefinition"/> to be satisfied by an <see cref="Export"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        ///     The property was not overridden by a derived class.
        /// </exception>
        /// <remarks>
        ///     <note type="inheritinfo">
        ///         Overriders of this property should never return <see langword="null"/>.
        ///     </note>
        /// </remarks>
        public virtual Expression<Func<ExportDefinition, bool>> Constraint
        {
            get
            {
                if (_constraint != null)
                {
                    return _constraint;
                }

                throw ExceptionBuilder.CreateNotOverriddenByDerived("Constraint");
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the import definition is required to be 
        ///     satisfied before a part can start producing exported objects.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if the <see cref="ImportDefinition"/> is required to be 
        ///     satisfied before a <see cref="ComposablePart"/> can start producing exported 
        ///     objects; otherwise, <see langword="false"/>. The default is <see langword="true"/>.
        /// </value>
        public virtual bool IsPrerequisite
        {
            get { return _isPrerequisite; }
        }

        /// <summary>
        ///     Gets a value indicating whether the import definition can be satisfied multiple times.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if the <see cref="ImportDefinition"/> can be satisfied 
        ///     multiple times throughout the lifetime of a <see cref="ComposablePart"/>, otherwise, 
        ///     <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        public virtual bool IsRecomposable
        {
            get { return _isRecomposable; }
        }

        /// <summary>
        ///     Returns a string representation of the import definition.
        /// </summary>
        /// <returns>
        ///     A <see cref="String"/> containing the value of the <see cref="Constraint"/> property.
        /// </returns>
        public override string ToString()
        {
            return this.Constraint.Body.ToString();
        }
    }
}
