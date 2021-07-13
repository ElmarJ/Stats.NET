using System;
using MathLib.Core.Analysis;
using MathLib.Core.Data;
using NGenerics.DataStructures.Mathematical;
using System.Linq;
using System.ComponentModel.Composition;

namespace MathLib.Modules.Analysis
{
    [Export(typeof(IAnalysis<ParametersPlaceholder, LinearRegressionResults>))]
    [Analysis("Performs Linear Regression Analysis", 1)]
    public class MultipleLinearRegressionAnalysis: Analysis<ParametersPlaceholder, LinearRegressionResults>
    {
        IVariable[] independentVariables;
        IVariable dependentVariable;
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

        public MultipleLinearRegressionAnalysis(IVariable dependentVariable, params IVariable[] independentVariables)
        {
            #region Validation
            
            if (dependentVariable == null)
                throw new ArgumentNullException("dependentVariable");
            if (independentVariables == null)
                throw new ArgumentNullException("independentVariables");
            

            foreach (Variable var in independentVariables)
            {
                if (dependentVariable.DataMatrix != var.DataMatrix)
                    throw new ArgumentException("Not all variables are from same DataSet");
            }

            #endregion

            this.independentVariables = (IVariable[])independentVariables.Clone();
            this.dependentVariable = dependentVariable;
            this.dataSet = this.dependentVariable.DataMatrix;
        }

        public override void Execute()
        {
            Compute();
        }

        private double SumOfProducts(IVariable variable1, IVariable variable2)
        {
            DataMatrix datamatrix = variable1.DataMatrix;
            double cov = (
                from r in datamatrix
                select (r[variable1].NummericalRepresentation * r[variable2].NummericalRepresentation))
                .Sum();
            return cov;
        }

        private void Compute()
        {
            // The X'X matrix:
            Matrix xTx = new Matrix(independentVariables.Length + 1, independentVariables.Length + 1);

            xTx[0, 0] = this.dataSet.Count;

            for (int i = 0; i < independentVariables.Length; i++)
            {
                DataMatrix datamatrix = independentVariables[i].DataMatrix;

                var variableSum = (
                    from r in datamatrix
                    select r[independentVariables[i]].NummericalRepresentation).Sum();
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
            DataMatrix datamatrix2 = dependentVariable.DataMatrix;

            var variableSum2 = (
                from r in datamatrix2
                select r[dependentVariable].NummericalRepresentation).Sum();

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



