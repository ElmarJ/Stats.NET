using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Data.Observations
{
    public interface ICategoricalObservation: IObservation
    {
        Category Value
        {
            get;
            set;
        }
    }
}
