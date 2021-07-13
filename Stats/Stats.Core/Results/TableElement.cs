using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Results.Presenters;

namespace Stats.Core.Results
{
    public class TableElement: Element
    {
        List<List<TableElementCell>> cells = new List<List<TableElementCell>>();
        List<TableElementRow> rows = new List<TableElementRow>();
        List<TableElementColumn> columns = new List<TableElementColumn>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional")]
        public TableElementCell this[TableElementRow row, TableElementColumn column]
        {
            get
            {
                int rowId = rows.BinarySearch(row);
                int colId = columns.BinarySearch(column);
                return cells[rowId][colId];
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional")]
        public TableElementCell this[int row, int column]
        {
            get
            {
                return cells[row][column];
            }
        }

        public TableElementColumnCollection Columns
        {
            get
            {
                return new TableElementColumnCollection(columns);
            }
        }

        public TableElementRowCollection Rows
        {
            get
            {
                return new TableElementRowCollection(rows);
            }
        }

        public TableElementRow AddRow()
        {
            // TODO: not thread-safe id!
            int rowId = rows.Count;
            TableElementRow row = new TableElementRow(this, rowId);
            rows.Add(row);

            this.cells.Add(new List<TableElementCell>());

            for (int columnId = 0; columnId < columns.Count; columnId++)
            {
                this.cells[rowId].Add(new TableElementCell());
            }
            return row;
        }

        public TableElementColumn AddColumn()
        {
            // TODO: not thread-safe!
            int columnId = columns.Count;
            TableElementColumn column = new TableElementColumn(this, columnId);
            columns.Add(column);

            for (int rowId = 0; rowId < rows.Count; rowId++)
            {
                this.cells[rowId].Add(new TableElementCell());
            }

            return column;
        }

        public override T Render<T>(IPresenter<T> presenter)
        {
            return presenter.RenderTableElement(this);
        }

        public string Title { get; set; }
    }
}
