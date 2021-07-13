using System.Collections.ObjectModel;
using System.Collections.Generic;
using Stats.Core.Environment;

namespace Stats.Core.Results
{
    public class ElementCollection: ProjectItemCollection<IElement>
    {
        public ElementCollection() :
            base()
        {
        }

        public ElementCollection(IList<IElement> list) :
            base((IList<IProjectItem>)list)
        {
        }

        public override string Name
        {
            get { return "ResultEllements"; }
        }
    }
}
