using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Results;

namespace Stats.Core.Analysis
{
    public interface IAnalysisFactory
    {
        IEnumerable<IAnalysis<IParameters, IResults>> Analysis { get; }
    }
}
