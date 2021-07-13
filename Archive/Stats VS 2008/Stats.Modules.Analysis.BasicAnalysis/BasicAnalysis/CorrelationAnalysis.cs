using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stats.Core.Analysis;
using Stats.Core.Data;
using System.Linq;
using System.ComponentModel.Composition;
using Stats.Core.Data.Observations;
using Stats.Core.Statistics;
using Stats.Core.Core.AddIns;

namespace Stats.Modules.Analysis
{
    [Export(typeof(IAnalysis)), AnalysisMetadata(1, "Correlation")]
    public class CorrelationAnalysis : Analysis<ParametersPlaceholder, CorrelationResults>
    {
        List<Variable> variables;
        CorrelationResults results;
        private int decimals = 3;

        public Collection<Variable> Variables
        {
            get { return new Collection<Variable>(variables); }
        }

        public CorrelationAnalysis()
        {
            this.variables = new List<Variable>();
        }

        public CorrelationAnalysis(IEnumerable<Variable> variables)
        {
            this.variables = new List<Variable>(variables);
            this.DataMatrix = variables.First().DataMatrix;
        }

        public CorrelationAnalysis(params Variable[] variables)
        {
            this.variables = new List<Variable>(variables);
            this.DataMatrix = variables.First().DataMatrix;
        }

        public override void Execute()
        {
            CorrelationCollection correlations =
                new CorrelationCollection();

            foreach (IVariable variable1 in this.variables)
            {
                correlations.Add(variable1, new Dictionary<IVariable, double>());
                foreach (IVariable variable2 in this.variables)
                {
                    correlations[variable1][variable2] = Math.Round(ComputePearsonsR(variable1, variable2), this.decimals);
                }
            }

            this.results = new CorrelationResults(correlations);
        }

        private double GetCovariance(IVariable variable1, IVariable variable2)
        {
            double productSum = (
                from r in this.DataMatrix.Records
                select (((INummericalObservation)r[(Variable)variable1]).Value * ((INummericalObservation)r[(Variable)variable2]).Value))
                .Sum();
            return productSum / this.DataMatrix.Records.Count;
        }

        //TODO: in C# 4.0 only accept IVariable<INummericalVariable>
        private double ComputePearsonsR(IVariable variable1, IVariable variable2)
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
