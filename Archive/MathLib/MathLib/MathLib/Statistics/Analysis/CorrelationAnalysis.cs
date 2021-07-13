using System;
using System.Collections.Generic;
using System.Text;
using MathLib.Statistics;
using System.Collections.ObjectModel;

namespace MathLib.Statistics.Analysis
{
    public class CorrelationAnalysis: IAnalysis
    {
        List<Variable> variables;
        CorrelationResults results;
        private int decimals = 3;

        public Collection<Variable> Variables
        {
            get { return new Collection<Variable>(variables); }
        }

        public CorrelationAnalysis(IEnumerable<Variable> variables)
        {
            this.variables = new List<Variable>(variables);
        }

        public CorrelationAnalysis(params Variable[] variables)
        {
            this.variables = new List<Variable>(variables);
        }

        #region IAnalysis Members

        public void Execute()
        {
            CorrelationCollection correlations =
                new CorrelationCollection();

            foreach (Variable variable in this.variables)
            {
                correlations.Add(variable, new Dictionary<Variable, double>());
                foreach (Variable variable2 in this.variables)
                {
                    correlations[variable][variable2] = Math.Round(variable.PearsonsR(variable2), this.decimals);
                }
            }

            this.results = new CorrelationResults(correlations);
        }

        public CorrelationResults Results
        {
            get { return this.results; }
        }

        Results IAnalysis.Results
        {
            get { return this.results; }
        }


        public int Decimals
        {
            get
            {
                return decimals;
            }
            set
            {
                decimals = value;
            }
        }

        #endregion
    }
}
