using System.Collections.Generic;
using Stats.Core.Data;
using Stats.Core.Data.Observations;

namespace Stats.Modules.Analysis
{
    public class CorrelationCollection : Dictionary<IVariable<IObservation>, Dictionary<IVariable<IObservation>, double>>
    {
    }
}
