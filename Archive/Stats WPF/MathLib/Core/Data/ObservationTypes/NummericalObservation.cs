using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.Core.Data.ObservationTypes
{
    public class NummericalObservation: OrdinalObservation
    {
        private double number;

        public NummericalObservation(Record record, Variable variable) :
            base(record, variable)
        {
            number = 0;
        }


        public NummericalObservation(Record record, Variable variable, double number) :
            base(record, variable)
        {
            this.number = number;
        }

        public override double NummericalRepresentation
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
            }
        }
    }
}
