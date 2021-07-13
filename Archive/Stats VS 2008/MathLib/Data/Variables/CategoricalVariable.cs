using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data.Observations;

namespace Stats.Core.Data
{
    class CategoricalVariable : Variable<CategoricalObservation>
    {
        CategoricalVariable()
            :base()
        {
            Categories = new CategoryDictionary();
        }

        public CategoryDictionary Categories
        {
            get;
            private set;
        }

        public override CategoricalObservation TypedNewObservation()
        {
            return new CategoricalObservation(this.Categories);
        }
    }
}
