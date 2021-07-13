using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;

namespace MathLib.Core.Data
{
    public class DataMatrixTable : IDataMatrix, INotifyPropertyChanged
    {
        private ObservableCollection<IVariable> variables = new ObservableCollection<IVariable>();
        private ObservableCollection<IRecord> records = new ObservableCollection<IRecord>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<IVariable> Variables
        {
            get
            {
                return this.variables;
            }
        }


        public ObservableCollection<IRecord> Records
        {
            get
            {
                return this.records;
            }
        }

        public ICollectionView CreateView()
        {
            return new MatrixDataView(this);
        }
    }
}
