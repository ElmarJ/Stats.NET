using System;
using MathLib.Core.Analysis;
using MathLib.Core.Results;
using System.ComponentModel.Composition;

namespace MathLib.Modules.Analysis
{
    [Export(typeof(IAnalysis<ParametersPlaceholder, DescriptivesResults>))]
    [Analysis("Produces Descriptives", 1)]
    public class DescriptivesAnalysis : IAnalysis<ParametersPlaceholder, DescriptivesResults>
    {
        public void Execute()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public DescriptivesResults Results
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        public int Decimals
        {
            get;
            set;
        }

        public ParametersPlaceholder Parameters
        {
            get;
            set;
        }

        public void Execute(MathLib.Core.Data.ICalculator<ParametersPlaceholder, DescriptivesResults> calculator)
        {
            throw new NotImplementedException();
        }

        #region IAnalysis Members


        IResults IAnalysis.Results
        {
            get { throw new NotImplementedException(); }
        }

        IParameters IAnalysis.Parameters
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
