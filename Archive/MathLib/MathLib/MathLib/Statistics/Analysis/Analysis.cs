using System;
using System.Collections.Generic;
using System.Text;

namespace MathLib.Statistics.Analysis
{
    public abstract class Analysis: IAnalysis
    {
        private int numberOfDecimals;

        public int NumberOfDecimals
        {
            get { return numberOfDecimals; }
            set { numberOfDecimals = value; }
        }

        #region IAnalysis Members

        #endregion
    }
}
