using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.Core.Data.ObservationTypes
{
    public class OrdinalObservation: CategoricalObservation
    {
        public OrdinalObservation(Record record, Variable variable) :
            base(record, variable)
        {
        }
    }
}
