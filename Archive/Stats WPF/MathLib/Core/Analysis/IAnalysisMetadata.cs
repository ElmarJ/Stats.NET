using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.Core.Analysis
{
    interface IAnalysisMetadata
    {
        string Description { get; }
        int Priority { get; }
    }
}
