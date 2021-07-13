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

        IObservation this[Variable variable]
        {
            get;
        }

        IObservation this[string variable]
        {
            get;
        }
    }
}
