using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.Core.Data
{
    public class Descriptives
    {
        private Variable variable;

        internal Descriptives(Variable variable)
        {
            this.variable = variable;
        }

        public double Mean
        {
            get
            {
                return variable.Sum / variable.DataMatrix.Count;
            }
        }

        public double StandardDeviation
        {
            get
            {
                return Math.Sqrt(this.Variance);
            }
        }

        public double MeanOfSquares
        {
            get
            {
                return variable.SumOfSquares / variable.DataMatrix.Count;
            }
        }

        public double SumOfSquares
        {
            get { return variable.SumOfSquares; }
        }

        public double Variance
        {
            get
            {
                return this.MeanOfSquares - (this.Mean * this.Mean);
            }
        }

        public double Sum
        {
            get { return variable.Sum; }
        }
    }
}
