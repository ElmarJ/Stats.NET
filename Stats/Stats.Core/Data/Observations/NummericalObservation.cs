using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Data.Observations
{
    public class NummericalObservation: Observation, INummericalObservation
    {
        private double number = 0;

        public NummericalObservation()
        {
        }

        public NummericalObservation(double number)
        {
            this.number = number;
        }

        public double Value
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

        public override string ToString()
        {
            return number.ToString();
        }
    }
}
