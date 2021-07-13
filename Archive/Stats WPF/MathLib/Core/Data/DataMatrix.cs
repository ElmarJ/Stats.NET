using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System;

namespace MathLib.Core.Data
{
    public class DataMatrix: ObservableCollection<IRecord>, IDataMatrix
    {
        ObservableCollection<IVariable> variables = new ObservableCollection<IVariable>();
      
        public DataMatrix()
        {
        }

        public virtual ObservableCollection<IVariable> Variables
        {
            get { return this.variables; }
        }

        #region IDataMatrix Members

        public virtual IObservation this[int index, IVariable variable]
        {
            get
            {
                return this[index][variable];
            }
        }

        public virtual IObservation this[int recordIndex, int variableIndex]
        {
            get
            {
                return this[recordIndex][variableIndex];
            }
        }

        #endregion

    }
}
