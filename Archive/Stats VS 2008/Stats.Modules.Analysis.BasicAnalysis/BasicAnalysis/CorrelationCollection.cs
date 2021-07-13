using System.Collections.Generic;
using Stats.Core.Data;

namespace Stats.Modules.Analysis
{
    public class CorrelationCollection : Dictionary<IVariable, Dictionary<IVariable, double>>
    {
    }
}
