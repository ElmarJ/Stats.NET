using System.Collections.Generic;
using MathLib.Core.Data.ObservationTypes;
using System.Collections.ObjectModel;
using System.Linq;

namespace MathLib.Core.Data
{
    public class Record: IRecord
    {
        private ObservableCollection<IVariable> variables;
        private Dictionary<IVariable, IObservation> observations = new Dictionary<IVariable, IObservation>();

        public Record(DataMatrix matrix)
        {
            this.variables = matrix.Variables;

            foreach (Variable variable in variables)
            {
                this.observations.Add(variable, variable.NewObservation(this));
            }
        }

        public Record(DataMatrix matrix, params double[] values)
            : this(matrix)
        {
            int i = 0;
            foreach (double value in values)
            {
                observations[variables[i++]].NummericalRepresentation = value;
            }
        }

        public IObservation this[IVariable variable]
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
                IVariable variable = (
                    from v in this.variables
                    where (v.Name == variableName)
                    select v).First();
                return observations[variable];
            }
        }
    }
}
