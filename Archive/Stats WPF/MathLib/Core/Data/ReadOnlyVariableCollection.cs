using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MathLib.Core.Data
{
    public class ReadOnlyVariableCollection : ReadOnlyObservableCollection<IVariable>
    {
        internal ReadOnlyVariableCollection(ObservableCollection<IVariable> variableCollection)
            : base(variableCollection)
        {
        }
    }
}
