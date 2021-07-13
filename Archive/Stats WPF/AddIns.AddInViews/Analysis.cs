using System.AddIn.Pipeline;

namespace AddIns.AddInViews
{
    [AddInBase]
    public abstract class Analysis
    {
        abstract public void Execute();

        abstract public MathLib.Core.Results.IResults Results
        {
            get;
        }

        abstract public MathLib.Core.Analysis.IParameters Parameters
        {
            get;
        }

        abstract public int Decimals
        {
            get;
            set;
        }
    }
}
