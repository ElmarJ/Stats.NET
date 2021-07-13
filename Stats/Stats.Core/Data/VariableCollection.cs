using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Stats.Core.Data.Observations;
using Stats.Core.Environment;

namespace Stats.Core.Data
{
    public class VariableCollection : ProjectItemCollection<IVariable<IObservation>>
    {
        IDataMatrix dataMatrix;

        internal VariableCollection(IDataMatrix dataMatrix)
            : base()
        {
            this.dataMatrix = dataMatrix;
        }

        protected override void InsertItem(int index, IVariable<IObservation> item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ArgumentException("Variable must have a name.");
            if ((from variable in this.dataMatrix.Variables select variable.Name).Contains(item.Name))
                throw new ArgumentException("Variable name already exists.");

            item.DataMatrix = this.dataMatrix;
            base.InsertItem(index, item);
        }

        public T Add<T>(string name)
            where T : IVariable<IObservation>, new()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Variable must have a name.");
            }
            T variable = new T();
            variable.Name = name;
            this.Add(variable);
            return variable;
        }

        public override string Name
        {
            get { return "Variables"; }
        }
    }
}
