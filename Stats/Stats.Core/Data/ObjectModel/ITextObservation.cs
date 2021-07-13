using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Data.Observations;

namespace Stats.Core.Data.ObjectModel
{
    public interface ITextObservation: IObservation
    {
        string Value
        {
            get;
            set;
        }
    }
}
