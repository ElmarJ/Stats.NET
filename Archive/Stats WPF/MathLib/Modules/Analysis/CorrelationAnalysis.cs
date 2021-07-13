using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MathLib.Core.Analysis;
using MathLib.Core.Data;
using System.Linq;
using System.ComponentModel.Composition;

namespace MathLib.Modules.Analysis
{
    [Export(typeof(IAnalysis<ParametersPlaceholder, CorrelationResults>))]
    [Analysis("Produces a Correlation Table", 1)]
    public class CorrelationAnalysis : Analysis<ParametersPlaceholder, CorrelationResults>
    {
        List<IVariable> variables;
        CorrelationResults results;
        private int decimals = 3;

        public Collection<IVariable> Variables
        {
            get { return new Collection<IVariable>(variables); }
        }

        public CorrelationAnalysis(IEnumerable<IVariable> variables)
        {
            this.variables = new List<IVariable>(variables);
        }

        public CorrelationAnalysis(params IVariable[] variables)
        {
            this.variables = new List<IVariable>(variables);
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
            DataMatrix datamatrix = variable1.DataMatrix;
            double productSum = (
                from r in datamatrix
                select (r[variable1].NummericalRepresentation * r[variable2].NummericalRepresentation))
                .Sum();
            return productSum / datamatrix.Count;
        }

        private double ComputePearsonsR(IVariable variable1, IVariable variable2)
        {
            return this.GetCovariance(variable1, variable2)
                / (variable1.Descriptives.StandardDeviation * variable2.Descriptives.StandardDeviation);
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
