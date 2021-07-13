using System;
using System.Data;
using System.Xml.Serialization;
using MathLib.Core.Data.ObservationTypes;

namespace MathLib.Core.Data.CachedData
{
    [Serializable]
    public abstract class CachedVariable<T>: Variable<T>
    {
        double sumOfSquares;
        double sum;
        string name;
        [NonSerialized] DataColumn column;
        bool keepUpToDate;
        [NonSerialized] DataMatrix dataMatrix;

        public CachedVariable()
        {
        }

        public CachedVariable(DataColumn column, DataMatrix dataMatrix)
        {
            this.column = column;
            this.KeepUpToDate = true;
            this.dataMatrix = dataMatrix;
            this.name = column.Caption;
        }

        public abstract IObservation NewObservation(Record record);

        public DataMatrix DataMatrix
        {
            get { return dataMatrix; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

//                if (this.dataMatrix != null)
//                    this.dataMatrix.Variables.Remove(this); 

                dataMatrix = value;
//                dataMatrix.Variables.Add(this);
            }
        }

        private void OnNewRow(object sender, DataTableNewRowEventArgs e)
        {
            this.AddObservation((double)e.Row[this.column]);
        }

        // Remove obeservations (before row is changed):
        private void OnRowChanging(object sender, DataRowChangeEventArgs e)
        {
            switch (e.Action)
            {
                case DataRowAction.Delete:
                    this.RemoveObservation((double)e.Row[this.column]);
                    break;
                case DataRowAction.Change:
                    this.RemoveObservation((double)e.Row[this.column]);
                    break;
            }
        }

        // Add observations (after row is changed):
        private void OnRowChanged(object sender, DataRowChangeEventArgs e)
        {
            switch (e.Action)
            {
                case DataRowAction.Change:
                    this.AddObservation((double)e.Row[this.column]);
                    break;
                case DataRowAction.Add:
                    this.AddObservation((double)e.Row[this.column]);
                    break;
            }
        }

        [XmlIgnore]
        internal double Sum
        {
            get { return this.sum; }
        }

        [XmlIgnore]
        public DataColumn Column
        {
            get { return this.column; }
            set { this.column = value; }
        }

        [XmlAttribute]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                if (this.column != null)
                    this.column.Caption = this.name;
            }
        }

        internal void AddObservation(double value)
        {
            this.sum += value;
            this.sumOfSquares += value * value;
        }

        internal void RemoveObservation(double value)
        {
            this.sum -= value;
            this.sumOfSquares -= value * value;
        }

        internal double SumOfSquares
        {
            get { return this.sumOfSquares; }
        }

        bool KeepUpToDate
        {
            set
            {
                bool newUpToDate = value;
                if (newUpToDate)
                {
                    this.UpdateStatistics();
                    this.column.Table.RowChanging += new DataRowChangeEventHandler(this.OnRowChanging);
                    this.column.Table.RowChanged += new DataRowChangeEventHandler(this.OnRowChanged);
                }
                else
                {
                    this.column.Table.RowChanging -= new DataRowChangeEventHandler(this.OnRowChanging);
                    this.column.Table.RowChanged -= new DataRowChangeEventHandler(this.OnRowChanged);
                }
                keepUpToDate = newUpToDate;
            }
            get
            {
                return this.keepUpToDate;
            }
        }

        public void UpdateStatistics()
        {
            // If statistics are automatically updated, they are already up-to-date:
            if (this.keepUpToDate) { return; }

            foreach (DataRow row in this.column.Table.Rows)
            {
                this.AddObservation((double)row[this.column]);
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public Descriptives Descriptives
        {
            get { return new Descriptives(this); }
        }
    }
}
