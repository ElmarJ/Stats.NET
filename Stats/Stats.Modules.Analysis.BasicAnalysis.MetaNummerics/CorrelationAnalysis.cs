using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stats.Core.Analysis;
using Stats.Core.Data;
using System.Linq;
using System.ComponentModel.Composition;
using Stats.Core.Data.Observations;
using Stats.Core.Statistics;
using Stats.Core.AddIns;
using Stats.Core.Results;

namespace Stats.Modules.Analysis
{
    [Export(typeof(IAnalysis<IParameters, IResults>)), AnalysisMetadata(1, "Correlation")]
    public class CorrelationAnalysis : Analysis<ParametersPlaceholder, CorrelationResults>
    {
        List<IVariable<IObservation>> variables;
        CorrelationResults results;
        private int decimals = 3;

        public Collection<IVariable<IObservation>> Variables
        {
            get { return new Collection<IVariable<IObservation>>(variables); }
        }

        public CorrelationAnalysis()
        {
            this.variables = new List<IVariable<IObservation>>();
        }

        public CorrelationAnalysis(IEnumerable<IVariable<IObservation>> variables)
        {
            this.variables = new List<IVariable<IObservation>>(variables);
            this.DataMatrix = variables.First().DataMatrix;
        }

        public CorrelationAnalysis(params IVariable<IObservation>[] variables)
        {
            this.variables = new List<IVariable<IObservation>>(variables);
            this.DataMatrix = variables.First().DataMatrix;
        }

        public override void Execute()
        {
            CorrelationCollection correlations =
                new CorrelationCollection();

            foreach (IVariable<INummericalObservation> variable1 in this.variables)
            {
                correlations.Add(variable1, new Dictionary<IVariable<IObservation>, double>());
                foreach (IVariable<INummericalObservation> variable2 in this.variables)
                {
                    correlations[variable1][variable2] = Math.Round(ComputePearsonsR(variable1, variable2), this.decimals);
                }
            }

            this.results = new CorrelationResults(correlations);
        }

        private double GetCovariance(IVariable<INummericalObservation> variable1, IVariable<INummericalObservation> variable2)
        {
            double productSum = (
                from r in this.DataMatrix.Records
                select (((INummericalObservation)r[(IVariable<IObservation>)variable1]).Value * ((INummericalObservation)r[(IVariable<IObservation>)variable2]).Value))
                .Sum();
            return productSum / this.DataMatrix.Records.Count;
        }

        private double ComputePearsonsR(IVariable<INummericalObservation> variable1, IVariable<INummericalObservation> variable2)
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
