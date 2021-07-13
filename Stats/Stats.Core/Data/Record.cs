using System.Collections.Generic;
using Stats.Core.Data.Observations;
using System.Collections.ObjectModel;
using System.Linq;

namespace Stats.Core.Data
{
    public class Record: IRecord
    {
        private VariableCollection variables;
        private Dictionary<IVariable<IObservation>, IObservation> observations = new Dictionary<IVariable<IObservation>, IObservation>();

        public Record(IDataMatrix matrix)
        {
            this.variables = matrix.Variables;

            foreach (var variable in variables)
            {
                this.observations.Add(variable, variable.NewObservation());
            }
        }

        public Record(IDataMatrix matrix, params IObservation[] observations)
            : this(matrix)
        {
            int i = 0;
            foreach (var observation in observations)
            {
                this.observations[variables[i++]] = observation;
            }
        }

        public Record(IDataMatrix matrix, params double[] observationValues)
            : this(matrix)
        {
            int i = 0;
            foreach (var val in observationValues)
            {
                this.observations[variables[i++]]
                    = new NummericalObservation(val);
            }
        }

        public Record(IDataMatrix matrix, params string[] observationValues)
            : this(matrix)
        {
            int i = 0;
            foreach (var val in observationValues)
            {
                this.observations[variables[i++]]
                    = new TextObservation(val);
            }
        }

        public IObservation this[IVariable<IObservation> variable]
        {
            get
            {
                return observations[variable];
            }
            set
            {
                observations[variable] = value;
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
                IVariable<IObservation> variable = (
                    from v in this.variables
                    where (v.Name == variableName)
                    select v).First();
                return observations[variable];
            }
        }

        public IEnumerator<IObservation> GetEnumerator()
        {
            foreach (KeyValuePair<IVariable<IObservation>, IObservation> kvp in this.observations)
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
