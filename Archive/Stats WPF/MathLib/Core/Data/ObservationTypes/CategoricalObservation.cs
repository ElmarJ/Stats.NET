using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.Core.Data.ObservationTypes
{
    public class CategoricalObservation: Observation
    {
        public CategoricalObservation(Record record, Variable variable)
            : base(record, variable)
        {
        }

        public override double NummericalRepresentation
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
