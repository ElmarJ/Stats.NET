using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data.Observations;
using Stats.Core.Core.Data;

namespace Stats.Core.Data
{
    public interface IVariable
    {
        string Name { get; }
        IEnumerable<IObservation> Observations { get; }
        Density Density { get; }
        MeasurementLevel MeasurementLevel { get; }

        IObservation NewObservation();
    }

    // T will be marked as "out" in c# 4.0 to support covariance
    public interface IVariable<T>: IVariable
        where T: IObservation
    {
        new IEnumerable<T> Observations { get; }

        T TypedNewObservation();
    }
}
