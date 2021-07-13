using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathLib.Core.Results;
using MathLib.Core.Analysis;

namespace MathLib.Core.Data
{
    public interface ICalculator<P, R>
        where P: IParameters
        where R: IResults
    {
        R Execute(P parameters);
    }
}
