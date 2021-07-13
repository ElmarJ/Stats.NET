using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Data.Observations
{
    public class CategoricalObservation: Observation, ICategoricalObservation
    {
        CategoryDictionary categories;
        int numValue;

        internal CategoricalObservation(CategoryDictionary categories)
        {
            this.categories = categories;
        }

        public Category Value
        {
            get
            {
                return categories[numValue];
            }
            set
            {
                SetCategory(value);
            }
        }

        private void SetCategory(Category category)
        {
            var keys =
                from c in categories
                where c.Value == category
                select c.Key;
            if (keys.Count() < 1)
            {
                throw new InvalidOperationException();
            }
            else
            {
                numValue = keys.First();
            }
        }
    }
}
