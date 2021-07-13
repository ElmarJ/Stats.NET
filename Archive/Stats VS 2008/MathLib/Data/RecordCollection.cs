using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data;
using System.Collections.ObjectModel;

namespace Stats.Core.Data
{
    public class RecordCollection: ObservableCollection<Record>
    {
        IDataMatrix dataMatrix;

        internal RecordCollection(IDataMatrix dataMatrix) :
            base()
        {
            this.dataMatrix = dataMatrix;
        }
    }
}
