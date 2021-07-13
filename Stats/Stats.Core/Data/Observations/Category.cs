using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Data.Observations
{
    public sealed class Category
    {
        Dictionary<int, string> dictionary;

        public int Value { get; private set; }
        public string ValueLabel
        {
            get
            {
                return this.dictionary[this.Value];
            }
        }

        internal Category(int value, Dictionary<int, string> dictionary)
        {
            this.Value = value;
            this.dictionary = dictionary;
        }
        
        public string Name
        {
            get;
            set;
        }
    }
}
