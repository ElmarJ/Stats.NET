using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Analysis
{
    public interface IAnalysisFactory
    {
        IAnalysis Analysis { get; }
    }
}
