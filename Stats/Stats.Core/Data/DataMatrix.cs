using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System;
using System.ComponentModel;
using System.Collections.Specialized;
using Stats.Core.Environment;
using System.Linq;

namespace Stats.Core.Data
{
    public abstract class DataMatrix: IDataMatrix, IProjectItemContainer //, ICollectionViewFactory
    {
        VariableCollection variables;
        RecordCollection records;

        protected DataMatrix()
        {
            this.variables = new VariableCollection(this);
            this.records = new RecordCollection();
        }

        public virtual VariableCollection Variables
        {
            get { return this.variables; }
        }

        public RecordCollection Records
        {
            get { return this.records; }
        }

        public string Name
        {
            get
            {
                return "Datamatrix";
            }
        }


        public IEnumerable<IProjectItem> SubItems
        {
            get { return this.Variables; }
        }
    }
}
