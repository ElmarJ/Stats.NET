using System;
using System.Collections.Generic;
using System.Text;
using MathLib.Collections;
using MathLib.Core.Data;
using MathLib.Core.Results;
using NGenerics.DataStructures.Mathematical;
using System.ComponentModel.Composition;

namespace MathLib.Modules.Analysis
{
    /// <summary>
    /// Contains the results of an Linear Regression Analysis.
    /// </summary>
    [Export(typeof(IResults))]
    public class LinearRegressionResults: Results
    {
        double constant;
        Dictionary<IVariable, double> coefficients = new Dictionary<IVariable, double>();
        double rSquare;
        IVariable dependentVariable;
        IVariable[] independentVariables;

        public IVariable DependentVariable
        {
            get { return dependentVariable; }
            set { this.dependentVariable = value; }
        }

        public IVariable[] IndependentVariables
        {
            get { return independentVariables; }
            set { this.independentVariables = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegressionResults"/> class.
        /// </summary>
        /// <param name="resultMatrix">The result matrix.</param>
        /// <param name="independentVariables">The independent variable-array used in the regression.</param>
        /// <param name="rSquare">The R-square value.</param>
        internal LinearRegressionResults(IVariable dependentVariable, IVariable[] independentVariables, Matrix resultMatrix, double rSquare, int decimals)
        {
            this.dependentVariable = dependentVariable;
            this.independentVariables = independentVariables;

            constant = Math.Round(resultMatrix[0, 0], decimals);

            for (int i = 0; i < independentVariables.Length; i++)
            {
                coefficients[independentVariables[i]] = Math.Round(resultMatrix[i + 1, 0], decimals);
            }

            this.rSquare = Math.Round(rSquare, decimals);
        }

        public LinearRegressionResults()
        {
        }

        /// <summary>
        /// Gets the constant or intercept of the regression-equation.
        /// </summary>
        /// <value>The constant.</value>
        public double Constant
        {
            get { return this.constant; }
            set { this.constant = value; }
        }

        /// <summary>
        /// Gets the coefficients.
        /// </summary>
        /// <value>The coefficients.</value>
        public Dictionary<IVariable, double> Coefficients
        {
            get { return this.coefficients; }
            set { this.coefficients = value; }
        }

        /// <summary>
        /// Gets the R square.
        /// </summary>
        /// <value>The R square.</value>
        public double RSquare
        {
            get { return rSquare; }
            set { rSquare = value; }
        }

        public string RegressionFormula
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("{0} = {1:n2}", dependentVariable, constant);

                bool isFirst = true;
                foreach (Variable var in independentVariables)
                {
                    if (!isFirst && coefficients[var] >= 0) builder.Append(" +");
                    if (coefficients[var] < 0) builder.Append(" -");

                    builder.AppendFormat(" {0:n2} x {1}", Math.Abs(coefficients[var]), var);
                    isFirst = false;
                }

                return builder.ToString();
            }
        }

        public override ElementCollection Elements
        {
            get
            {
                ElementCollection elements = new ElementCollection();

                elements.Add(new TextElement("Formula: " + RegressionFormula));
                elements.Add(new TextElement("R-square: " + this.RSquare.ToString()));

                TableElement table = new TableElement();
                table.Title = "Regression Coefficients";

                table.AddColumn();

                table.AddRow();
                table.Rows[0].Header = "(Constant)";
                table[0, 0].Value = this.Constant;

                foreach (KeyValuePair<IVariable, double> kvp in this.Coefficients)
                {
                    table.AddRow();
                    table.Rows[table.Rows.Count - 1].Header = kvp.Key.Name;
                    table[table.Rows.Count - 1, 0].Value = kvp.Value;
                }

                elements.Add(table);

                return elements;
            }
        }
    }
}