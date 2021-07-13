using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data.Observations;

namespace Stats.Core.Data.Variables
{
    public class NummericalVariable: Variable<NummericalObservation>
    {
        public override NummericalObservation TypedNewObservation()
        {
            return new NummericalObservation();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
