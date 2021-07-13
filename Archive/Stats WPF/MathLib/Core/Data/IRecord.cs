using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.Core.Data
{
    public interface IRecord
    {
        IObservation this[int index]
        {
            get;
        }

        IObservation this[IVariable variable]
        {
            get;
        }

        IObservation this[string variable]
        {
            get;
        }
    }
}
