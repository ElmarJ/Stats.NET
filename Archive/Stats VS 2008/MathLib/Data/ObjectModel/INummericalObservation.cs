using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Data.Observations
{
    public interface INummericalObservation: IObservation, IOrdinalObservation
    {
        double Value
        {
            get;
            set;
        }
    }
}
