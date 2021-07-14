using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stats.Core.Analysis;
using Stats.Core.Data;
using System.Linq;
using System.ComponentModel.Composition;
using Stats.Core.Data.Observations;
using Stats.Core.Statistics;
using MathLib.Modules.Analysis;

namespace Stats.Modules.Analysis
{
    [Export(typeof(IAnalysis<ParametersPlaceholder, CorrelationResults>))]
    [Analysis("Produces a Correlation Table", 1)]
    public class CorrelationAnalysis : Analysis<ParametersPlaceholder, CorrelationResults>
    {
        List<Variable<IObservation>> variables;
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

        public override void Execute()
        {
            CorrelationCollection correlations =
                new CorrelationCollection();

            foreach (Variable<INummericalObservation> variable1 in this.variables)
            {
                correlations.Add(variable1, new Dictionary<IVariable, double>());
                foreach (Variable<INummericalObservation> variable2 in this.variables)
                {
                    correlations[variable1][variable2] = Math.Round(ComputePearsonsR(variable1, variable2), this.decimals);
                }
            }

            this.results = new CorrelationResults(correlations);
        }

        private double GetCovariance(Variable variable1, Variable variable2)
        {
            double productSum = (
                from r in this.DataMatrix.Records
                select (((INummericalObservation)r[variable1]).Value * ((INummericalObservation)r[variable2]).Value))
                .Sum();
            return productSum / this.DataMatrix.Records.Count;
        }

        private double ComputePearsonsR(Variable<INummericalObservation> variable1, Variable<INummericalObservation> variable2)
        {
            return this.GetCovariance(variable1, variable2)
                / (variable1.StandardDeviation() * variable2.StandardDeviation());
        }

        public override CorrelationResults Results
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
    }
}
