using System;
using Stats.Core.Analysis;
using Stats.Core.Data;
using System.Linq;
using System.ComponentModel.Composition;
using Stats.Core.Data.Observations;
using Stats.Core.Results;
using MathNet.Numerics.LinearAlgebra;

namespace Stats.Modules.Analysis
{
    [Export(typeof(IAnalysis<IParameters, IResults>))]
    [Analysis("Performs Linear Regression Analysis", 1)]
    public class LinearRegressionAnalysis: Analysis<LinearRegressionParameters, LinearRegressionResults>
    {
        IVariable<IObservation>[] independentVariables;
        IVariable<IObservation> dependentVariable;
        LinearRegressionResults results;

        public override LinearRegressionResults Results
        {
            get
            {
                if (this.results == null) throw new InvalidOperationException();
                return results;
            }
        }

        public LinearRegressionAnalysis()
        {
        }

        public LinearRegressionAnalysis(IVariable<IObservation> dependentVariable, params IVariable<IObservation>[] independentVariables) :
            this(new LinearRegressionParameters(dependentVariable, independentVariables))
        {
        }

        public LinearRegressionAnalysis(LinearRegressionParameters parameters)
        {
            #region Validation

            if (parameters.DependentVariable == null)
                throw new ArgumentNullException("dependentVariable");
            if (parameters.IndependentVariables == null)
                throw new ArgumentNullException("independentVariables");

            this.DataMatrix = parameters.DependentVariable.DataMatrix;

            foreach (IVariable<IObservation> var in parameters.IndependentVariables)
            {
                if (!this.DataMatrix.Variables.Contains(var))
                    throw new ArgumentException("Not all variables are in DataSet");
            }

            #endregion

            this.Parameters = parameters;
            this.independentVariables = (IVariable<IObservation>[])independentVariables.Clone();
            this.dependentVariable = parameters.DependentVariable;
        }

        public override void Execute()
        {
            Compute();
        }

        private double SumOfProducts(IVariable<IObservation> variable1, IVariable<IObservation> variable2)
        {
            double cov = (
                from r in this.DataMatrix.Records
                select (((INummericalObservation)r[variable1]).Value * ((INummericalObservation)r[variable2]).Value))
                .Sum();
            return cov;
        }

        private void Compute()
        {
            // The X'X matrix:
            
            var xTx = Matrix<double>.Build.Dense(independentVariables.Length + 1, independentVariables.Length + 1);

            xTx[0, 0] = this.DataMatrix.Records.Count;

            for (int i = 0; i < independentVariables.Length; i++)
            {

                var variableSum = (
                    from r in this.DataMatrix.Records
                    select ((INummericalObservation)r[independentVariables[i]]).Value).Sum();
                xTx[0, i + 1] = xTx[i + 1, 0] = variableSum;
            }

            for (int i = 0; i < independentVariables.Length; i++)
            {
                for (int j = 0; j < independentVariables.Length; j++)
                {
                    xTx[i + 1, j + 1] = xTx[j + 1, i + 1] = SumOfProducts(independentVariables[i], independentVariables[j]);
                }
            }

            // The X'Y matrix:
            var xTy = Vector<double>.Build.Dense(independentVariables.Length + 1);

            var variableSum2 = (
                from r in this.DataMatrix.Records
                select ((INummericalObservation)r[dependentVariable]).Value).Sum();

            xTy[0] = variableSum2;

            for (int i = 0; i < independentVariables.Length; i++)
            {
                xTy[i + 1] =
                    Math.Round(
                    SumOfProducts(independentVariables[i], dependentVariable),
                    this.Parameters.Decimals);
            }

            // Calculate the estimates (beta est = X'X.inv * X'Y):
            var resultMatrix = xTx.Inverse() * xTy;

            this.results = new LinearRegressionResults(
                this.dependentVariable,
                this.independentVariables,
                resultMatrix,
                0,
                this.Parameters.Decimals);
        }
    }
}



