using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data;
using System.Collections.ObjectModel;
using Stats.Core.Environment;
using System.Data;

namespace Stats.Core.Data
{
    public class RecordCollection : ObservableCollection<Record>
    {
        internal RecordCollection() :
            base()
        {
        }

    }
}
