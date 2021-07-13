using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System;
using System.Collections;
using System.Linq;

namespace Stats.Core.Data
{
    public interface IDataMatrix
    {
        RecordCollection Records
        {
            get;
        }

        VariableCollection Variables
        {
            get;
        }
    }
}
