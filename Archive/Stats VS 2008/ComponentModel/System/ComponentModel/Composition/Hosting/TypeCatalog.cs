// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.AttributedModel;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Hosting
{
    /// <summary>
    ///     An immutable ComposablePartCatalog created from a type array or a list of managed types.  This class is threadsafe.
    ///     It is Disposable.
    /// </summary>
    [DebuggerTypeProxy(typeof(ComposablePartCatalogDebuggerProxy))]
    public class TypeCatalog : ComposablePartCatalog, ICompositionElement
    {
        private readonly object _thisLock = new object();
        private Type[] _types = null;
        private volatile IQueryable<ComposablePartDefinition> _queryableParts;
        private volatile bool _isDisposed = false;
        private readonly ICompositionElement _definitionOrigin;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeCatalog"/> class 
        ///     with the specified types.
        /// </summary>
        /// <param name="types">
        ///     An <see cref="Array"/> of attributed <see cref="Type"/> objects to add to the 
        ///     <see cref="TypeCatalog"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="types"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="types"/> contains an element that is <see langword="null"/>.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="types"/> contains an element that was loaded in the Reflection-only context.
        /// </exception>
        public TypeCatalog(params Type[] types)
            : this(types, (ICompositionElement)null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeCatalog"/> class
        ///     with the specified types.
        /// </summary>
        /// <param name="types">
        ///     An <see cref="IEnumerable{T}"/> of attributed <see cref="Type"/> objects to add 
        ///     to the <see cref="TypeCatalog"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="types"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="types"/> contains an element that is <see langword="null"/>.
        ///     <para>
        ///         -or-
        ///     </para>
        ///     <paramref name="types"/> contains an element that was loaded in the reflection-only context.
        /// </exception>
        public TypeCatalog(IEnumerable<Type> types)
            : this(types, (ICompositionElement)null)
        {
        }

        internal TypeCatalog(IEnumerable<Type> types, ICompositionElement definitionOrigin)
        {
            Requires.NotNull(types, "types");

            foreach (Type type in types)
            {
                if (type == null)
                {
                    throw ExceptionBuilder.CreateContainsNullElement("types");
                }
#if !SILVERLIGHT
                if (type.Assembly.ReflectionOnly)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.Argument_ElementReflectionOnlyType, "types"), "types");
                }
#endif
            }

            this._types = types.ToArray();
            this._definitionOrigin = definitionOrigin ?? this;
        }


        /// <summary>
        ///     Gets the part definitions of the catalog.
        /// </summary>
        /// <value>
        ///     A <see cref="IQueryable{T}"/> of <see cref="ComposablePartDefinition"/> objects of the 
        ///     <see cref="TypeCatalog"/>.
        /// </value>
        /// <exception cref="ObjectDisposedException">
        ///     The <see cref="TypeCatalog"/> has been disposed of.
        /// </exception>
        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                this.ThrowIfDisposed();

                return this.PartsInternal;
            }
        }

        /// <summary>
        ///     Gets the display name of the type catalog.
        /// </summary>
        /// <value>
        ///     A <see cref="String"/> containing a human-readable display name of the <see cref="TypeCatalog"/>.
        /// </value>
        string ICompositionElement.DisplayName
        {
            get { return this.GetDisplayName(); }
        }

        /// <summary>
        ///     Gets the composition element from which the type catalog originated.
        /// </summary>
        /// <value>
        ///     This property always returns <see langword="null"/>.
        /// </value>
        ICompositionElement ICompositionElement.Origin
        {
            get { return null; }
        }

        private IQueryable<ComposablePartDefinition> PartsInternal
        {
            get
            {
                if (this._queryableParts == null)
                {
                    lock (this._thisLock)
                    {
                        if (this._queryableParts == null)
                        {
                            Assumes.NotNull(this._types);

                            var collection = new List<ComposablePartDefinition>();
                            foreach (Type type in this._types)
                            {
                                var definition = AttributedModelDiscovery.CreatePartDefinitionIfDiscoverable(type, _definitionOrigin);
                                if (definition != null)
                                {
                                    collection.Add(definition);
                                }
                            }
                            IQueryable<ComposablePartDefinition> queryableParts = collection.AsQueryable();
                            Thread.MemoryBarrier();

                            this._types = null;
                            this._queryableParts = queryableParts;
                        }
                    }
                }

                return this._queryableParts;
            }
        }


        /// <summary>
        ///     Returns a string representation of the type catalog.
        /// </summary>
        /// <returns>
        ///     A <see cref="String"/> containing the string representation of the <see cref="TypeCatalog"/>.
        /// </returns>
        public override string ToString()
        {
            return this.GetDisplayName();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._isDisposed = true;
            }

            base.Dispose(disposing);
        }

        private string GetDisplayName()
        {
            return String.Format(CultureInfo.CurrentCulture, 
                                 "{0} (Types=\"{1}\")",     // NOLOC
                                 this.GetType().Name, 
                                 this.GetTypesDisplay());
        }

        private string GetTypesDisplay()
        {
            int count = this.PartsInternal.Count();
            if (count == 0)
            {
                return Strings.TypeCatalog_Empty;
            }

            const int displayCount = 2;
            StringBuilder builder = new StringBuilder();
            foreach (ReflectionComposablePartDefinition definition in this.PartsInternal.Take(displayCount))
            {
                if (builder.Length > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(definition.GetPartType().GetDisplayName());
            }

            if (count > displayCount)
            {   // Add an elipse to indicate that there 
                // are more types than actually listed

                builder.Append(", ...");
            }
            
            return builder.ToString();
        }

        private void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw ExceptionBuilder.CreateObjectDisposed(this);
            }
        }
    }
}
