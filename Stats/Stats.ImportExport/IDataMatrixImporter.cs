using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data;

namespace Stats.Interoperability
{
    interface IDataMatrixImporter
    {
        IDataMatrix Import(System.IO.Stream importStream);
    }
}
