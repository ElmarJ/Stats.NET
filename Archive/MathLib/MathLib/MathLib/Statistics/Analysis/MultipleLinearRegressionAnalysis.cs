using System;
using System.Collections.Generic;
using System.Text;
using MathLib.Statistics;
using NGenerics.DataStructures;

namespace MathLib.Statistics.Analysis
{
    public class MultipleLinearRegressionAnalysis: IAnalysis
    {
        Variable[] independentVariables;
        Variable dependentVariable;
        DataSet dataSet;
        RegressionResults results;
        private int decimals = 3;

        public RegressionResults Results
        {
            get
            {
                if (this.results == null) throw new InvalidOperationException();
                return results;
            }
        }

        public MultipleLinearRegressionAnalysis(Variable dependentVariable, params Variable[] independentVariables)
        {
            if (dependentVariable == null)
                throw new ArgumentNullException("dependentVariable");
            if (independentVariables == null)
                throw new ArgumentNullException("independentVariables");
            foreach (Variable var in independentVariables)
            {
                if (dependentVariable.DataSet != var.DataSet)
                    throw new ArgumentException("Not all variables are from same DataSet");
            }

            this.independentVariables = (Variable[])independentVariables.Clone();
            this.dependentVariable = dependentVariable;
            this.dataSet = this.dependentVariable.DataSet;
        }

        public void Execute()
        {
            // The X'X matrix:
            Matrix xTx = new Matrix(independentVariables.Length + 1, independentVariables.Length + 1);
            
            xTx[0, 0] = this.dataSet.CaseCount;

            for (int i = 0; i < independentVariables.Length; i++)
            {
                xTx[0, i + 1] =
                    Math.Round(
                    independentVariables[i].Sum,
                    this.decimals);
                xTx[i + 1, 0] =
                    Math.Round(
                    independentVariables[i].Sum,
                    this.decimals);
            }

            for (int i = 0; i < independentVariables.Length; i++)
            {
                for (int j = 0; j < independentVariables.Length; j++)
                {
                    xTx[i, j] =
                        Math.Round(
                        independentVariables[i].ProductSum(independentVariables[j]),
                        this.decimals);
                }
            }

            // The X'Y matrix:
            Matrix xTy = new Matrix(independentVariables.Length + 1, 1);
            xTy[0, 0] = dependentVariable.Sum;

            for (int i = 0; i < independentVariables.Length; i++)
            {
                xTy[i + 1, 0] =
                    Math.Round(
                    independentVariables[i].ProductSum(dependentVariable),
                    this.decimals);
            }

            // Calculate the estimates (beta est = X'X.inv * X'Y):
            Matrix resultMatrix = xTx.Invert() * xTy;

            this.results = new RegressionResults(
                this.dependentVariable,
                this.independentVariables,
                resultMatrix,
                0,
                this.decimals);
        }

        #region IAnalysis Members

        Results IAnalysis.Results
        {
            get
            {
                return this.results;
            }
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



