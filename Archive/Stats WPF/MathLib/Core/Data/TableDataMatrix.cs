using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace MathLib.Core.Data
{
    public class TableDataMatrix: DataMatrix, IEnumerable<DataRow>
    {
        System.Data.DataTable dataTable;
        List<Variable> autoUpdateCoproducts = new List<Variable>();
        double[,] coproductSums;
        VariableCollection variables = new VariableCollection();
        bool keepUpToDate;

        public bool KeepUpToDate
        {
            get { return keepUpToDate; }
            set
            {
                bool newKeepUpToDate = value;
                if (newKeepUpToDate)
                {
                    this.UpdateStatistics();
                    this.dataTable.RowChanged += new DataRowChangeEventHandler(dataTable_RowChanged);
                    this.dataTable.RowChanging += new DataRowChangeEventHandler(dataTable_RowChanging);
                }
                else
                {
                    this.dataTable.RowChanged -= new DataRowChangeEventHandler(dataTable_RowChanged);
                    this.dataTable.RowChanging -= new DataRowChangeEventHandler(dataTable_RowChanging);
                }
                keepUpToDate = value;
            }
        }

        public TableDataMatrix(System.Data.DataTable dataTable)
        {
            this.dataTable = dataTable;
            this.KeepUpToDate = true;
            this.InitializeVariables();
            this.dataTable.Columns.CollectionChanged +=new System.ComponentModel.CollectionChangeEventHandler(Columns_CollectionChanged);
        }

        public TableDataMatrix()
            : this(new System.Data.DataTable())
        {
        }

        void dataTable_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            switch (e.Action)
            {
                case DataRowAction.Change:
                    this.RemoveCoproducts(e.Row);
                    break;
                case DataRowAction.Delete:
                    this.RemoveCoproducts(e.Row);
                    break;
            }
        }

        void RemoveCoproducts(DataRow dataRow)
        {
            foreach (Variable var in autoUpdateCoproducts)
            {
                for (int i = 0; i < var.Column.Ordinal; i++)
                {
                    coproductSums[var.Column.Ordinal, i] -=
                        (double)dataRow[var.Column] * (double)dataRow[i];
                }
            }
        }

        void dataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            switch (e.Action)
            {
                case DataRowAction.Add:
                    this.AddCoproducts(e.Row);
                    break;
                case DataRowAction.Change:
                    this.AddCoproducts(e.Row);
                    break;
            }
        }

        void AddCoproducts(DataRow dataRow)
        {
            foreach (Variable var in autoUpdateCoproducts)
            {
                for (int i = 0; i < var.Column.Ordinal; i++)
                {
                    coproductSums[var.Column.Ordinal, i] +=
                        (double)dataRow[var.Column] * (double)dataRow[i];
                }
            }
        }

        void InitializeVariables()
        {
            foreach (DataColumn column in dataTable.Columns)
            {
                variables.Add(new Variable(column, this));
            }
        }

        public void UpdateStatistics()
        {
            if (keepUpToDate) return;

            coproductSums = new double[dataTable.Columns.Count, dataTable.Columns.Count];

            foreach (DataRow row in dataTable.Rows)
            {
                AddCoproducts(row);
            }
        }

        public VariableCollection Variables
        {
            get { return (VariableCollection)this.variables; }
        }

        void Columns_CollectionChanged(object sender, System.ComponentModel.CollectionChangeEventArgs e)
        {
            DataColumn column = (DataColumn)e.Element;
            switch (e.Action)
            {
                case System.ComponentModel.CollectionChangeAction.Add:
                    this.variables.Add(new Variable(column, this));
                    break;
                case System.ComponentModel.CollectionChangeAction.Remove:
                    foreach (Variable variable in this.variables)
                    {
                        if (variable.Column == column)
                        {
                            variables.Remove(variable);
                            break;
                        }
                    }
                    break;
            }
        }
        internal double GetCovariance(Variable variable1, Variable variable2)
        {
            if (variable1.DataMatrix != variable2.DataMatrix)
                throw new System.InvalidOperationException();
            return
                SumOfProducts(variable1, variable2) / this.CaseCount - variable1.Descriptives.Mean * variable2.Descriptives.Mean;
        }
        public int CaseCount
        {
            get
            {
                return this.dataTable.Rows.Count;
            }
        }

        public double SumOfProducts(Variable variable1, Variable variable2)
        {
            if (variable1.DataMatrix != variable2.DataMatrix)
                throw new System.InvalidOperationException();
            if(variable1 == variable2)
                return variable1.SumOfSquares;

            if (this.autoUpdateCoproducts.Contains(variable1) &&
                this.autoUpdateCoproducts.Contains(variable2))
            {
                if (variable1.Column.Ordinal > variable2.Column.Ordinal)
                {
                    return this.coproductSums[variable1.Column.Ordinal, variable2.Column.Ordinal];
                }
                else
                {
                    return this.coproductSums[variable2.Column.Ordinal, variable1.Column.Ordinal];
                }
            }
            else
            {
                return ComputeSumOfProducts(variable1, variable2);
            }
        }

        double ComputeSumOfProducts(Variable variable1, Variable variable2)
        {
            double sum = 0;
            foreach (DataRow row in this.dataTable.Rows)
            {
                sum += (double)row[variable1.Column] * (double)row[variable2.Column];
            }

            return sum;
        }

        #region IEnumerable<DataRow> Members

        public IEnumerator<DataRow> GetEnumerator()
        {
            foreach (DataRow row in this.dataTable.Rows)
            {
                yield return row;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.dataTable.Rows.GetEnumerator();
        }

        #endregion
    }
}
