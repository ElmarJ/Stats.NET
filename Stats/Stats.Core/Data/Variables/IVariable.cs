using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data.Observations;
using Stats.Core.Core.Data;
using Stats.Core.Environment;

namespace Stats.Core.Data
{
    public interface IVariable<out T>: IProjectItem
        where T: IObservation
    {
        IEnumerable<T> Observations { get; }

        new string Name { get; set; }
        Density Density { get; }
        MeasurementLevel MeasurementLevel { get; }
        IDataMatrix DataMatrix { get; set; }

        T NewObservation();
    }
}
