using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml.Serialization;

namespace MathLib.Statistics
{
    [Serializable]
    public class Variable
    {
        double sumOfSquares;
        double sum;
        string name;
        [NonSerialized] DataColumn column;
        bool keepUpToDate;
        [NonSerialized] DataSet dataSet;

        public Variable()
        {
        }

        public Variable(DataColumn column, DataSet dataSet)
        {
            this.column = column;
            this.KeepUpToDate = true;
            this.dataSet = dataSet;
            this.name = column.Caption;
        }

        internal DataSet DataSet
        {
            get { return dataSet; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                
                if (this.dataSet != null)
                    this.dataSet.Variables.Remove(this); 

                dataSet = value;
                dataSet.Variables.Add(this);
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

        double MeanOfSquares
        {
            get
            {
                return this.SumOfSquares / this.Count;
            }
        }

        [XmlIgnore]
        public double StandardDeviation
        {
            get
            {
                return Math.Sqrt(this.Variance);
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
        public double Sum
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

        [XmlIgnore]
        public double Mean
        {
            get
            {
                return this.sum / this.column.Table.Rows.Count;
            }
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

        double Count
        {
            get { return this.column.Table.Rows.Count; }
        }

        [XmlIgnore]
        public double Variance
        {
            get
            {
                return this.MeanOfSquares - (this.Mean * this.Mean);
            }
        }
        
        public double PearsonsR(Variable variable)
        {
            if (variable == null)
                throw new ArgumentNullException("variable");
            return this.Covariance(variable)
                / (this.StandardDeviation * variable.StandardDeviation);
        }

        public double Covariance(Variable variable)
        {
            return this.dataSet.GetCovariance(this, variable);
        }

        internal double ProductSum(Variable variable)
        {
            return this.dataSet.GetSumOfProducts(this, variable);
        }

        public override string ToString()
        {
            return this.column.ToString();
        }
    }
}
