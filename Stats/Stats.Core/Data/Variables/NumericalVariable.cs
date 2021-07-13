using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data.Observations;

namespace Stats.Core.Data.Variables
{
    public class NumericalVariable: Variable<INummericalObservation>
    {
        public override INummericalObservation NewObservation()
        {
            return new NummericalObservation();
        }

        public override string ToString()
        {
            return this.Name;
        }

        public new IEnumerable<INummericalObservation> Observations
        {
            get { return base.Observations; }
        }
    }
}
