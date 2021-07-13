using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Stats.Core.Results
{
    public class ElementCollection: Collection<IElement>
    {
        public ElementCollection() :
            base()
        {
        }

        public ElementCollection(IList<IElement> list) :
            base(list)
        {
        }
    }
}
