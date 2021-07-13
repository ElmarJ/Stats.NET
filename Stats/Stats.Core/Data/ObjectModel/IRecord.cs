using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data.Observations;

namespace Stats.Core.Data
{
    public interface IRecord: IEnumerable<IObservation>
    {
        IObservation this[int index]
        {
            get;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers")]
        IObservation this[IVariable<IObservation> variable]
        {
            get;
        }

        IObservation this[string variable]
        {
            get;
        }
    }
}
