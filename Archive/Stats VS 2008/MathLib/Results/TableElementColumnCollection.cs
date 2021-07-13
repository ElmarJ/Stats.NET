using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Stats.Core.Results
{
    public class TableElementColumnCollection: ReadOnlyCollection<TableElementColumn>
    {
        internal TableElementColumnCollection(IList<TableElementColumn> columnList) :
            base(columnList)
        {
        }
    }
}
