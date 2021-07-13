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
    [Serializable]
    public abstract class Variable<T> : IVariable<T>
        where T : IObservation
    {
        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                // Name cannot be empty:
                if (String.IsNullOrEmpty(value))
                    throw new System.NullReferenceException();

                // No duplicate variable names allowed:
                if (this.DataMatrix != null && (from var in this.DataMatrix.Variables select var.Name).Contains(value))
                    throw new System.InvalidOperationException();

                this.name = value;
            }
        }

        public IDataMatrix DataMatrix
        {
            get;
            set;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public IEnumerable<T> Observations
        {
            get
            {
                IVariable<IObservation> thisVariable = (IVariable<IObservation>)this;
                foreach (IRecord record in this.DataMatrix.Records)
                {
                    yield return (T)record[thisVariable];
                }
            }
        }

        public abstract T NewObservation();

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
