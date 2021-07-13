using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Results;

namespace Stats.Core.Analysis.Interfaces
{
    //TODO: Obsolete?
    public interface IInterface<T> where T:IAnalysis<IParameters, IResults>
    {
        
    }
}
