using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Stats.Core.Data.Observations
{
    // Todo: why does it have the serializable attribute???
    [Serializable]
    public class CategoryDictionary: Dictionary<int, Category>
    {
    }
}
