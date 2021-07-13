using System;
using MathLib.Core.Analysis;
using MathLib.Core.Data;
using System.Linq;
using System.ComponentModel.Composition;
using MathLib.Core.Data.Observations;
using NGenerics.DataStructures.Mathematical;

namespace MathLib.Modules.Analysis
{
    [Export(typeof(IAnalysis<ParametersPlaceholder, LinearRegressionResults>))]
    [Analysis("Performs Linear Regression Analysis", 1)]
    public class MultipleLinearRegressionAnalysis: Analysis<ParametersPlaceholder, LinearRegressionResults>
    {
        Variable[] independentVariables;
        Variable dependentVariable;
        DataMatrix dataSet;
        LinearRegressionResults results;
        private int decimals = 3;

        public override LinearRegressionResults Results
        {
            get
            {
                if (this.results == null) throw new InvalidOperationException();
                return results;
            }
        }

        public MultipleLinearRegressionAnalysis(Variable dependentVariable, params Variable[] independentVariables)
        {
            #region Validation
            
            if (dependentVariable == null)
                throw new ArgumentNullException("dependentVariable");
            if (independentVariables == null)
                throw new ArgumentNullException("independentVariables");
            

            foreach (Variable var in independentVariables)
            {
                if (!this.DataMatrix.Variables.Contains(var))
                    throw new ArgumentException("Not all variables are in DataSet");
            }

            #endregion

            this.independentVariables = (Variable[])independentVariables.Clone();
            this.dependentVariable = dependentVariable;
        }

        public override void Execute()
        {
            Compute();
        }

        private double SumOfProducts(Variable variable1, Variable variable2)
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
            Matrix xTx = new Matrix(independentVariables.Length + 1, independentVariables.Length + 1);

            xTx[0, 0] = this.DataMatrix.Records.Count;

            for (int i = 0; i < independentVariables.Length; i++)
            {

                var variableSum = (
                    from r in this.DataMatrix.Records
                    select ((INummericalObservation)r[independentVariables[i]]).Value).Sum();
                xTx[0, i + 1] = variableSum;
                xTx[i + 1, 0] = variableSum;
            }

            for (int i = 0; i < independentVariables.Length; i++)
            {
                for (int j = 0; j < independentVariables.Length; j++)
                {
                    xTx[i + 1, j + 1] =
                        SumOfProducts(independentVariables[i], independentVariables[j]);
                }
            }

            // The X'Y matrix:
            Matrix xTy = new Matrix(independentVariables.Length + 1, 1);

            var variableSum2 = (
                from r in this.DataMatrix.Records
                select ((INummericalObservation)r[dependentVariable]).Value).Sum();

            xTy[0, 0] = variableSum2;

            for (int i = 0; i < independentVariables.Length; i++)
            {
                xTy[i + 1, 0] =
                    Math.Round(
                    SumOfProducts(independentVariables[i], dependentVariable),
                    this.decimals);
            }

            // Calculate the estimates (beta est = X'X.inv * X'Y):
            Matrix resultMatrix = xTx.Inverse() * xTy;

            this.results = new LinearRegressionResults(
                this.dependentVariable,
                this.independentVariables,
                resultMatrix,
                0,
                this.decimals);
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



