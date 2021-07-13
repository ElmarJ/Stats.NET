// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using Microsoft.Internal;
using Microsoft.Internal.Collections;
using System.Collections;

namespace System.ComponentModel.Composition.Hosting
{
    /// <summary>
    ///     This class implements a threadsafe ICollection{T} of ComposablePartCatalog.
    ///     It is exposed as an ICollection(ComposablePartCatalog)
    ///     It is threadsafe, notifications are not marshalled using a SynchronizationContext.
    ///     It is Disposable.
    /// </summary>
    internal class ComposablePartCatalogCollection : ICollection<ComposablePartCatalog>, IDisposable
    {
        private readonly Lock _lock = new Lock();
        private EventHandler<ComposablePartCatalogChangedEventArgs> _collectionChangedNotification;
        private List<ComposablePartCatalog> _catalogs = new List<ComposablePartCatalog>();
        private volatile bool _isCopyNeeded = false;
        private volatile bool _isDisposed = false;
        private bool _hasChanged = false;

        public ComposablePartCatalogCollection(
            IEnumerable<ComposablePartCatalog> catalogs,
            EventHandler<ComposablePartCatalogChangedEventArgs> collectionChangedNotification)
        {
            // Need a lock and notifications
            Assumes.NotNull(collectionChangedNotification);
            catalogs = catalogs ?? Enumerable.Empty<ComposablePartCatalog>();

            this._catalogs = new List<ComposablePartCatalog>(catalogs);
            this._collectionChangedNotification = collectionChangedNotification;

            foreach (var item in catalogs.OfType<INotifyComposablePartCatalogChanged>())
            {
                item.Changed += this._collectionChangedNotification;               
            }
        }

        public void Add(ComposablePartCatalog item)
        {
            Requires.NotNull(item, "item");

            this.ThrowIfDisposed();

            INotifyComposablePartCatalogChanged notifyCatalog = item as INotifyComposablePartCatalogChanged;
            if (notifyCatalog != null)
            {
                notifyCatalog.Changed += this._collectionChangedNotification;
            }

            IEnumerable<ComposablePartDefinition> items = item.Parts.ToArray();

            using (new WriteLock(this._lock))
            {
                if (this._isCopyNeeded)
                {
                    this._catalogs = new List<ComposablePartCatalog>(this._catalogs);
                    this._isCopyNeeded = false;
                }
                this._hasChanged = true;
                this._catalogs.Add(item);
            }

            this._collectionChangedNotification(this, new ComposablePartCatalogChangedEventArgs(items));
        }

        public void Clear()
        {
            this.ThrowIfDisposed();

            // No action is required if we are already empty
            using (new ReadLock(this._lock))
            {
                if (this._catalogs.Count == 0)
                {
                    return;
                }
            }

            ComposablePartCatalog[] catalogs = null;
            using (new WriteLock(this._lock))
            {
                catalogs = this._catalogs.ToArray();
                this._catalogs = new List<ComposablePartCatalog>();

                this._isCopyNeeded = false;
                this._hasChanged = true;
            }


            // We are doing this outside of the lock, so it's possible that the catalog will continute propagatign events from things
            // we are about to unsubscribe from. Given the non-specificity of our event, in the worst case scenario we would simply fire 
            // unnecessary events.
            this._collectionChangedNotification(this, new ComposablePartCatalogChangedEventArgs(catalogs.SelectMany(catalog => catalog.Parts).ToArray()));
            this.UnsubscribeFromCatalogNotifications(catalogs);
        }

        public bool Contains(ComposablePartCatalog item)
        {
            Requires.NotNull(item, "item");

            this.ThrowIfDisposed();

            using (new ReadLock(this._lock))
            {
                return this._catalogs.Contains(item);
            }
        }

        public void CopyTo(ComposablePartCatalog[] array, int arrayIndex)
        {
            this.ThrowIfDisposed();

            using (new ReadLock(this._lock))
            {
                this._catalogs.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get 
            {
                this.ThrowIfDisposed();

                using (new ReadLock(this._lock))
                {
                    return this._catalogs.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                this.ThrowIfDisposed();

                return false;
            }
        }

        public bool Remove(ComposablePartCatalog item)
        {
            Requires.NotNull(item, "item");

            this.ThrowIfDisposed();

            bool isSuccessfulRemoval = false;

            INotifyComposablePartCatalogChanged notifyCatalog = item as INotifyComposablePartCatalogChanged;
            if (notifyCatalog != null)
            {
                notifyCatalog.Changed -= this._collectionChangedNotification;
            }

            IEnumerable<ComposablePartDefinition> items = item.Parts.ToArray();

            using (new WriteLock(this._lock))
            {
                if (_isCopyNeeded)
                {
                    this._catalogs = new List<ComposablePartCatalog>(this._catalogs);
                    this._isCopyNeeded = false;
                }

                isSuccessfulRemoval = this._catalogs.Remove(item);
                if (!isSuccessfulRemoval)
                {
                    // Return an empty list
                    items = Enumerable.Empty<ComposablePartDefinition>();
                }
                else
                {
                    this._hasChanged = true;
                }
            }

            this._collectionChangedNotification(this, new ComposablePartCatalogChangedEventArgs(items));
            return isSuccessfulRemoval;
        }

        internal bool HasChanged
        {
            get
            {
                this.ThrowIfDisposed();

                using (new ReadLock(this._lock))
                {
                    return this._hasChanged;
                }
            }
        }

        public IEnumerator<ComposablePartCatalog> GetEnumerator()
        {
            this.ThrowIfDisposed();

            using (new ReadLock(this._lock))
            {
                IEnumerator<ComposablePartCatalog> enumerator = this._catalogs.GetEnumerator();
                this._isCopyNeeded = true;
                return enumerator;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this._isDisposed)
                {
                    bool disposeLock = false;
                    IEnumerable<ComposablePartCatalog> catalogs = null;
                    try
                    {
                        using (new WriteLock(this._lock))
                        {
                            if (!this._isDisposed)
                            {
                                disposeLock = true;

                                catalogs = this._catalogs;
                                this._catalogs = null;

                                this._isDisposed = true;
                            }
                        }
                    }
                    finally
                    {
                        if (catalogs != null)
                        {
                            this.UnsubscribeFromCatalogNotifications(catalogs);
                            catalogs.ForEach(catalog => catalog.Dispose());
                        }

                        if (disposeLock)
                        {
                            this._lock.Dispose();
                        }
                    }
                }
            }
        }

        private void UnsubscribeFromCatalogNotifications(IEnumerable<ComposablePartCatalog> catalogs)
        {
            catalogs.OfType<INotifyComposablePartCatalogChanged>().ForEach(notifyCatalog =>
                {
                    notifyCatalog.Changed -= this._collectionChangedNotification;
                });
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
