using System.Collections.ObjectModel;
using Stats.Core.Data;
using System;

namespace Stats.Core.Environment
{
    [Serializable]
    public class DataSetCollection : ProjectItemCollection<IDataMatrix> 
    {
        public override string Name
        {
            get { return "Datasets"; }
        }
    }
}
