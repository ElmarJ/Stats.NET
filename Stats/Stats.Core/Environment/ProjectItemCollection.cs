using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;

namespace Stats.Core.Environment
{
    public abstract class ProjectItemCollection<T>: ObservableCollection<T>, IProjectItemContainer where T: IProjectItem
    {
        public ProjectItemCollection() :
            base()
        {
        }

        public ProjectItemCollection(IList<IProjectItem> list) :
            base((IList<T>)list)
        {
        }

        public IEnumerable<IProjectItem> SubItems
        {
            get { return (IEnumerable<IProjectItem>)this; }
        }

        public abstract string Name { get; }
    }
}
