using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data.Observations;

namespace Stats.Core.Data
{
    public class CategoricalVariable : Variable<CategoricalObservation>
    {
        CategoricalVariable()
            :base()
        {
            this.Categories = new CategoryDictionary();
        }

        public CategoryDictionary Categories
        {
            get;
            private set;
        }

        public override CategoricalObservation NewObservation()
        {
            return new CategoricalObservation(this.Categories);
        }

    }
}
