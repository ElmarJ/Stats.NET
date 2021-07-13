using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data.ObjectModel;

namespace Stats.Core.Data.Observations
{
    public class TextObservation : Observation, ITextObservation
    {
        public TextObservation()
        {
        }

        public TextObservation(string text)
        {
            this.Value = text;
        }

        public string Value
        {
            get;
            set;
        }
    }
}
