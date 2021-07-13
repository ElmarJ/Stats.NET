using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Stats.Core.Data
{
    class MatrixDataView: CollectionView, IItemProperties, IEditableCollectionView
    {
        public MatrixDataView(IDataMatrix dataMatrix)
            : base(new List<IRecord>())
        {

        }

        ReadOnlyCollection<ItemPropertyInfo> IItemProperties.ItemProperties
        {
            get
            {
                List<ItemPropertyInfo> properties = new List<ItemPropertyInfo>();
                foreach (Variable variable in ((DataMatrix)this.SourceCollection).Variables)
                {
                    properties.Add(new ItemPropertyInfo(variable.Name, variable.GetType(), new object()));
                }

                return new ReadOnlyCollection<ItemPropertyInfo>(properties);
            }
        }

        #region IEditableCollectionView Members

        public object AddNew()
        {
            throw new NotImplementedException();
        }

        public bool CanAddNew
        {
            get { throw new NotImplementedException(); }
        }

        public bool CanCancelEdit
        {
            get { throw new NotImplementedException(); }
        }

        public bool CanRemove
        {
            get { throw new NotImplementedException(); }
        }

        public void CancelEdit()
        {
            throw new NotImplementedException();
        }

        public void CancelNew()
        {
            throw new NotImplementedException();
        }

        public void CommitEdit()
        {
            throw new NotImplementedException();
        }

        public void CommitNew()
        {
            throw new NotImplementedException();
        }

        public object CurrentAddItem
        {
            get { throw new NotImplementedException(); }
        }

        public object CurrentEditItem
        {
            get { throw new NotImplementedException(); }
        }

        public void EditItem(object item)
        {
            throw new NotImplementedException();
        }

        public bool IsAddingNew
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsEditingItem
        {
            get { throw new NotImplementedException(); }
        }

        public NewItemPlaceholderPosition NewItemPlaceholderPosition
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Remove(object item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollectionView Members

        public bool CanFilter
        {
            get { throw new NotImplementedException(); }
        }

        public bool CanGroup
        {
            get { throw new NotImplementedException(); }
        }

        public bool CanSort
        {
            get { throw new NotImplementedException(); }
        }

        public bool Contains(object item)
        {
            throw new NotImplementedException();
        }

        public System.Globalization.CultureInfo Culture
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler CurrentChanged;

        public event CurrentChangingEventHandler CurrentChanging;

        public object CurrentItem
        {
            get { throw new NotImplementedException(); }
        }

        public int CurrentPosition
        {
            get { throw new NotImplementedException(); }
        }

        public IDisposable DeferRefresh()
        {
            throw new NotImplementedException();
        }

        public Predicate<object> Filter
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { throw new NotImplementedException(); }
        }

        public ReadOnlyObservableCollection<object> Groups
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsCurrentAfterLast
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsCurrentBeforeFirst
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsEmpty
        {
            get { throw new NotImplementedException(); }
        }

        public bool MoveCurrentTo(object item)
        {
            throw new NotImplementedException();
        }

        public bool MoveCurrentToFirst()
        {
            throw new NotImplementedException();
        }

        public bool MoveCurrentToLast()
        {
            throw new NotImplementedException();
        }

        public bool MoveCurrentToNext()
        {
            throw new NotImplementedException();
        }

        public bool MoveCurrentToPosition(int position)
        {
            throw new NotImplementedException();
        }

        public bool MoveCurrentToPrevious()
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public SortDescriptionCollection SortDescriptions
        {
            get { throw new NotImplementedException(); }
        }

        public System.Collections.IEnumerable SourceCollection
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }
}
