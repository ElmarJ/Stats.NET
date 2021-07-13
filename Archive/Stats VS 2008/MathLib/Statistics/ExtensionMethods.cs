using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data;
using Stats.Core.Data.Observations;

namespace Stats.Core.Statistics
{
    public static class ExtensionMethods
    {
        public static double StandardDeviation(this IVariable<INummericalObservation> var)
        {
            var variation =
                (from o in var.Observations
                 select o.Value * o.Value).Sum();
            var variance = variation / var.Observations.Count();
            var sd = Math.Sqrt(variance);
            return sd;
        }

        // Temporary method until C# 4 arrives
        public static double StandardDeviation(this IVariable var)
        {
            var variation =
                (from o in var.Observations
                 select ((INummericalObservation)o).Value * ((INummericalObservation)o).Value).Sum();
            var variance = variation / var.Observations.Count();
            var sd = Math.Sqrt(variance);
            return sd;
        }
    }
}
