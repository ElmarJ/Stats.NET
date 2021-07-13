using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Analysis;
using Stats.Core.Data;

namespace Stats.Modules.Analysis
{
    public class LinearRegressionParameters: Parameters
    {
        LinearRegressionParameters():
            base()
        {
            Decimals = 3;
        }

        public LinearRegressionParameters(Variable dependent, params Variable[] independent):
            this()
        {
            this.IndependentVariables = independent;
            this.DependentVariable = dependent;
        }

        public Variable[] IndependentVariables
        {
            get;
            set;
        }

        public Variable DependentVariable
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
