using System;
using Stats.Core.Results;
using Stats.Core.Data;

namespace Stats.Core.Analysis
{
    public abstract class Analysis<P, R>: IAnalysis, IAnalysis<P, R>
        where P: IParameters
        where R: IResults
    {

        IResults IAnalysis.Results
        {
            get
            {
                return this.Results;
            }
        }

        IParameters IAnalysis.Parameters
        {
            get
            {
                return this.Parameters;
            }
        }

        int IAnalysis.Decimals
        {
            get;
            set;
        }

        public virtual R Results
        {
            get;
            protected set;
        }

        public virtual P Parameters
        {
            get;
            set;
        }

        public abstract void Execute();

        public IDataMatrix DataMatrix
        {
            get;
            set;
        }
    }
}
