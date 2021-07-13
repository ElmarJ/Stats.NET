using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Data.Observations
{
    public sealed class Category
    {
        int value;
        Dictionary<int, string> dictionary;

        internal Category(int value, Dictionary<int, string> dictionary)
        {
            this.value = value;
            this.dictionary = dictionary;
        }

        public string Name
        {
            get;
            set;
        }
    }
}
