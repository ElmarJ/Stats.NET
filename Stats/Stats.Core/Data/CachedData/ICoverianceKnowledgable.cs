using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Data
{
    public interface ICoverianceKnowledgable: IDataMatrix
    {
        double Covariance<T>(IVariable<T> variable1, IVariable<T> variable2) where T: Observations.IObservation;
        double SumOfProducts<T>(IVariable<T> variable1, IVariable<T> variable2) where T : Observations.IObservation;
    }
}
