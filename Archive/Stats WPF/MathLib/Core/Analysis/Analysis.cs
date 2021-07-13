using System;
using MathLib.Core.Results;

namespace MathLib.Core.Analysis
{
    public abstract class Analysis<P, R>: IAnalysis, IAnalysis<P, R>
        where P: IParameters, new()
        where R: IResults, new()
    {
        private P parameters;
        private R results;

        public Analysis()
        {
            parameters = new P();
        }

        IResults IAnalysis.Results
        {
            get { return this.Results; }
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
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual R Results
        {
            get
            {
                return results;
            }
        }

        public virtual P Parameters
        {
            get
            {
                return parameters;
            }
        }

        public virtual void Execute(MathLib.Core.Data.ICalculator<P, R> calculator)
        {

            this.results = calculator.Execute(this.parameters);
        }

        public abstract void Execute();
    }
}
