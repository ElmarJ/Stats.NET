using System;
using System.Collections.Generic;
using System.Text;
using MathLib.Statistics;
using MathLib.Collections;
using System.Xml.Serialization;
using System.Xml.Xsl;
using NGenerics.DataStructures;

namespace MathLib.Statistics.Analysis
{
    /// <summary>
    /// Contains the results of an Linear Regression Analysis.
    /// </summary>
    [Serializable]
    public class RegressionResults: Results
    {
        double constant;
        MathLib.Collections.SerializableDictionary<Variable, double> coefficients = new SerializableDictionary<Variable,double>();
        double rSquare;
        Variable dependentVariable;
        Variable[] independentVariables;

        [XmlElement]
        public Variable DependentVariable
        {
            get { return dependentVariable; }
            set { this.dependentVariable = value; }
        }

        [XmlArray]
        [XmlArrayItem(typeof(Variable))]
        public Variable[] IndependentVariables
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
        internal RegressionResults(Variable dependentVariable, Variable[] independentVariables, Matrix resultMatrix, double rSquare, int decimals)
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

        public RegressionResults()
        {
        }

        /// <summary>
        /// Gets the constant or intercept of the regression-equation.
        /// </summary>
        /// <value>The constant.</value>
        [XmlElement]
        public double Constant
        {
            get { return this.constant; }
            set { this.constant = value; }
        }

        /// <summary>
        /// Gets the coefficients.
        /// </summary>
        /// <value>The coefficients.</value>
        [XmlElement]
        public SerializableDictionary<Variable, double> Coefficients
        {
            get { return this.coefficients; }
            set { this.coefficients = value; }
        }

        /// <summary>
        /// Gets the R square.
        /// </summary>
        /// <value>The R square.</value>
        [XmlElement]
        public double RSquare
        {
            get { return rSquare; }
            set { rSquare = value; }
        }

        [XmlElement]
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

        protected override XslCompiledTransform HtmlTransformation
        {
            get
            {
                System.Xml.Xsl.XslCompiledTransform transform = new System.Xml.Xsl.XslCompiledTransform();
                transform.Load("Statistics\\Analysis\\RegressionResultsToHtml.xslt");
                return transform;
            }
        }


        public override System.Windows.Documents.FlowDocument ToFlowDocument()
        {
            throw new NotImplementedException();
        }
    }
}