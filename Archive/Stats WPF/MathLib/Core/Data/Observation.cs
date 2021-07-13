using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.Core.Data
{
    public abstract class Observation: IObservation
    {
        private Record record;
        private Variable variable;

        internal Observation(Record record, Variable variable)
        {
            this.record = record;
            this.variable = variable;
        }

        protected Record Record
        {
            get { return this.record; }
        }

        protected Variable Variable
        {
            get { return this.variable; }
        }

        public abstract double NummericalRepresentation
        {
            get;
            set;
        }
    }
}
