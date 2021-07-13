using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Analysis;
using Stats.Core.Data;
using Stats.Core.Data.Observations;

namespace Stats.Modules.Analysis
{
    public class LinearRegressionParameters: Parameters
    {
        LinearRegressionParameters():
            base()
        {
            Decimals = 3;
        }

        public LinearRegressionParameters(IVariable<IObservation> dependent, params IVariable<IObservation>[] independent) :
            this()
        {
            this.IndependentVariables = independent;
            this.DependentVariable = dependent;
        }

        public IVariable<IObservation>[] IndependentVariables
        {
            get;
            set;
        }

        public IVariable<IObservation> DependentVariable
        {
            get;
            set;
        }

        public int Decimals
        {
            get;
            set;
        }
    }
}
