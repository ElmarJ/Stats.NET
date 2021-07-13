using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.Core.Results
{
    public sealed class TableElementColumn
    {
        private TableElement table;
        private int index;

        internal int Index
        {
            get
            {
                return this.index;
            }
        }

        public string Header { get; set; }
        public object Total { get; set; }
        public string TotalFormatString { get; set; }

        internal TableElementColumn(TableElement table, int index)
        {
            this.table = table;
            this.index = index;
        }

        public TableElementCell this[int row]
        {
            get
            {
                return this.table[row, index];
            }
        }
    }
}
