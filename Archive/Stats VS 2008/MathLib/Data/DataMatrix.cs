using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Stats.Core.Data
{
    public class DataMatrix: IDataMatrix //, ICollectionViewFactory
    {
        VariableCollection variables;
        RecordCollection records;

        public DataMatrix()
        {
            this.variables = new VariableCollection(this);
            this.records = new RecordCollection(this);
        }

        public virtual VariableCollection Variables
        {
            get { return this.variables; }
        }

        public RecordCollection Records
        {
            get { return this.records; }
        }

    }
}
