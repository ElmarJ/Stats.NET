using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Data.Observations
{
    public class OrdinalObservation: CategoricalObservation, IOrdinalObservation
    {
        internal OrdinalObservation(CategoryDictionary categories):
            base(categories)
        {
        }

        public int CompareTo(IOrdinalObservation other)
        {
            if (other == null)
            {
                return 1;
            }
            else if (other.GetType() != typeof(OrdinalObservation))
            {
                throw new InvalidOperationException();
            }
            else if (((INummericalObservation)other).Value > ((INummericalObservation)this).Value)
            {
                return -1;
            }
            else if (((INummericalObservation)other).Value == ((INummericalObservation)this).Value)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
