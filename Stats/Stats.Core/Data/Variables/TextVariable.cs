using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data.Observations;
using Stats.Core.Data.ObjectModel;

namespace Stats.Core.Data.Variables
{
        public class TextVariable : Variable<ITextObservation>
        {
            public override ITextObservation NewObservation()
            {
                return new TextObservation();
            }

            public override string ToString()
            {
                return this.Name;
            }

            public new IEnumerable<ITextObservation> Observations
            {
                get { return base.Observations; }
            }
    }
}
