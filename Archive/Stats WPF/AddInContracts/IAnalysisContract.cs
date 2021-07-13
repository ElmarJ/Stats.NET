using System.AddIn.Contract;
using MathLib.Core.Results;
using MathLib.Core.Analysis;
using System.AddIn.Pipeline;

namespace AddIns.Contracts
{
    [AddInContract]
    public interface IAnalysisContract: IContract
    {
        void Execute();
        IResults Results { get; }
        IParameters Parameters { get; }
        int Decimals { get; set; }
    }
}
