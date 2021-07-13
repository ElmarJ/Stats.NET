using System.AddIn.Pipeline;
using AddIns.Contracts;
using AddIns.AddInViews;

namespace AddIns.AddInAdapters
{
    public class AnalysisAddInViewToContractAdapter: ContractBase, IAnalysisContract
    {
        Analysis analysis;

        AnalysisAddInViewToContractAdapter(Analysis analysis)
        {
            this.analysis = analysis;
        }

        public void Execute()
        {
            analysis.Execute();
        }

        public MathLib.Core.Results.IResults Results
        {
            get { return this.analysis.Results; }
        }

        public MathLib.Core.Analysis.IParameters Parameters
        {
            get { return this.analysis.Parameters; }
        }

        public int Decimals
        {
            get
            {
                return this.analysis.Decimals;
            }
            set
            {
                this.analysis.Decimals = value;
            }
        }
    }
}
