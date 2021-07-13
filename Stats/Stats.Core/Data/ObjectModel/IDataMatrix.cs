using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System;
using System.Collections;
using System.Linq;
using Stats.Core.Environment;

namespace Stats.Core.Data
{
    public interface IDataMatrix: IProjectItemContainer
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
