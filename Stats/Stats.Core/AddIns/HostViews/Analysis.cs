using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathLib.Core.Analysis;
using MathLib.Core.Results;

namespace MathLib.Core.AddIns.HostViews
{
    public abstract class Analysis : IAnalysis
    {
        abstract public void Execute();

        abstract public IResults Results
        {
            get;
        }

        abstract public IParameters Parameters
        {
            get;
        }

        abstract public int Decimals
        {
            get;
            set;
        }
    }
}
