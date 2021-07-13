using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathLib.Core.Analysis;
using MathLib.Core.Results;

namespace MathLib.Core.Data
{
    public abstract class Calculator<P, R>: ICalculator<P, R>
        where P: IParameters
        where R: IResults
    {
        public R Execute(P parameters)
        {
            throw new NotImplementedException();
        }
    }
}
