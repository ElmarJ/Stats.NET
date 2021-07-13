using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Stats.Core.Data.Observations;

namespace Stats.Core.Data
{
    public class VariableCollection : ObservableCollection<Variable>
    {
        IDataMatrix dataMatrix;

        internal VariableCollection(IDataMatrix dataMatrix)
            : base()
        {
            this.dataMatrix = dataMatrix;
        }

        protected override void InsertItem(int index, Variable item)
        {
            if (item.Name == String.Empty)
                throw new ArgumentException();
            if ((from variable in this.dataMatrix.Variables select variable.Name).Contains(item.Name))
                throw new ArgumentException();

            item.DataMatrix = this.dataMatrix;
            base.InsertItem(index, item);
        }

        public Variable Add<T>(string name)
            where T: Variable, new()
        {
            Variable variable = new T();
            variable.Name = name;
            this.Add(variable);
            return variable;
        }
    }
}
