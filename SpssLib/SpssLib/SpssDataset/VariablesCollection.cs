using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SpssLib.SpssDataset
{
    public class VariablesCollection: Collection<Variable>
    {
        public VariablesCollection()
            : base()
        {
        }

        public VariablesCollection(IList<Variable> list)
            : base(list)
        {
        }
    }
}
