using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data;

namespace Stats.Interoperability.DataBoundDataMatrix
{
    class DataBoundDataMatrix: DataMatrix
    {
        public Object DataSource { get; set; }
    }
}
