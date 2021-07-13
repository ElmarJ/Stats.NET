using System;
using System.Collections.Generic;
using System.Text;

namespace MathLib.Statistics.Analysis
{
    public class DescriptivesAnalysis : IAnalysis
    {
        #region IAnalysis Members

        public void Execute()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public Results Results
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        private int decimals;

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
