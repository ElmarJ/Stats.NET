using System.Collections.Generic;
using Stats.Core.Data.Observations;
using System.Collections.ObjectModel;
using System.Linq;

namespace Stats.Core.Data
{
    public class Record: IRecord
    {
        private VariableCollection variables;
        private Dictionary<Variable, IObservation> observations = new Dictionary<Variable, IObservation>();

        public Record(DataMatrix matrix)
        {
            this.variables = matrix.Variables;

            foreach (Variable variable in variables)
            {
                this.observations.Add(variable, variable.NewObservation());
            }
        }

        public Record(DataMatrix matrix, params IObservation[] observations)
            : this(matrix)
        {
            int i = 0;
            foreach (IObservation observation in observations)
            {
                this.observations[variables[i++]] = observation;
            }
        }

        public Record(DataMatrix matrix, params double[] observationValues)
            : this(matrix)
        {
            int i = 0;
            foreach (double val in observationValues)
            {
                this.observations[variables[i++]]
                    = new NummericalObservation(val);
            }
        }

        public IObservation this[Variable variable]
        {
            get
            {
                return observations[variable];
            }
        }

        public IObservation this[int index]
        {
            get
            {
                return observations[variables[index]];
            }
        }

        public IObservation this[string variableName]
        {
            get
            {
                Variable variable = (
                    from v in this.variables
                    where (v.Name == variableName)
                    select v).First();
                return observations[variable];
            }
        }

        public IEnumerator<IObservation> GetEnumerator()
        {
            foreach (KeyValuePair<Variable, IObservation> kvp in this.observations)
            {
                yield return kvp.Value;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
