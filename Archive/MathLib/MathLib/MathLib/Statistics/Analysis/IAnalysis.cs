using System;
using System.Collections.Generic;
using System.Text;

namespace MathLib.Statistics.Analysis
{

    /// <summary>
    /// Defines methods
    /// </summary>
    public interface IAnalysis
    {
        void Execute();
        Results Results { get; }
        int Decimals { get; set; }
    }
}
