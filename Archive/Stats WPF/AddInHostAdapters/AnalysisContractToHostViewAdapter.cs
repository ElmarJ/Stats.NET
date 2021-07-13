using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn.Pipeline;
using MathLib.Core.AddIns.HostViews;
using AddIns.Contracts;
using MathLib.Core.Results;

namespace AddIns.HostAdapters
{
    [HostAdapter]
    public class AnalysisContractToHostViewAdapter: Analysis
    {
        private IAnalysisContract contract;
        private ContractHandle handle;

        public AnalysisContractToHostViewAdapter(IAnalysisContract contract)
        {
            this.contract = contract;
            handle = new ContractHandle(contract);
        }

        public override void Execute()
        {
            contract.Execute();
        }

        public override IResults Results
        {
            get { return this.contract.Results; }
        }

        public override MathLib.Core.Analysis.IParameters Parameters
        {
            get { return this.contract.Parameters; }
        }

        public override int Decimals
        {
            get
            {
                return this.contract.Decimals;
            }
            set
            {
                this.contract.Decimals = value;
            }
        }
    }
}
