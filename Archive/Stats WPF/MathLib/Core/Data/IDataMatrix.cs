using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System;
using System.Collections;
using System.Linq;

namespace MathLib.Core.Data
{
    public interface IDataMatrix: IEnumerable<IRecord>
    {
        IObservation this[int recordIndex, int variableIndex]
        {
            get;
        }

        IRecord this[int recordIndex]
        {
            get;
        }

        ObservableCollection<IVariable> Variables
        {
            get;
        }

        int Count
        {
            get;
        }
    }
}
