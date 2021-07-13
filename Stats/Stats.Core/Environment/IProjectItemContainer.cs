using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Environment
{
    public interface IProjectItemContainer: IProjectItem
    {
        IEnumerable<IProjectItem> SubItems { get; }
    }
}
