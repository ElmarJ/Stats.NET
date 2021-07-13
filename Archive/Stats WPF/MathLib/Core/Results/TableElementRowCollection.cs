using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MathLib.Core.Results
{
    public class TableElementRowCollection: ReadOnlyCollection<TableElementRow>
    {
        internal TableElementRowCollection(IList<TableElementRow> rowList) :
            base(rowList)
        {
        }
    }
}
