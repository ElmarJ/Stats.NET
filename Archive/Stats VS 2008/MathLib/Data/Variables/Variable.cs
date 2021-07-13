using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml.Serialization;
using Stats.Core.Data.Observations;
using Stats.Core.Core.Data;

namespace Stats.Core.Data
{
    public abstract class Variable
    {
        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                // Name cannot be empty:
                if (value == String.Empty || value == null)
                    throw new System.ArgumentException();
                
                // No duplicate variable names allowed:
                if (this.DataMatrix != null && (from var in this.DataMatrix.Variables select var.Name).Contains(value))
                    throw new System.ArgumentException();
                
                this.name = value;
            }
        }

        public IDataMatrix DataMatrix
        {
            get;
            internal set;
        }

        public abstract IObservation NewObservation();
    }

    [Serializable]
    public abstract class Variable<T> : Variable, IVariable<T>
        where T : IObservation
    {
        public IEnumerable<T> Observations
        {
            get
            {
                foreach (IRecord record in this.DataMatrix.Records)
                {
                    yield return (T)record[this];
                }
            }
        }

        IEnumerable<IObservation> IVariable.Observations
        {
            get
            {
                foreach (IRecord record in this.DataMatrix.Records)
                {
                    yield return (T)record[this];
                }
            }
        }

        public abstract T TypedNewObservation();

        public override IObservation NewObservation()
        {
            return this.TypedNewObservation();
        }

        public virtual Density Density
        {
            get { return Density.Continuous; }
        }

        public virtual MeasurementLevel MeasurementLevel
        {
            get { return MeasurementLevel.Ordinal; }
        }

    }
}
