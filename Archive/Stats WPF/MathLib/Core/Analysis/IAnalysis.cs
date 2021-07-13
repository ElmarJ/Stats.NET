using MathLib.Core.Results;
namespace MathLib.Core.Analysis
{

    /// <summary>
    /// Defines methods
    /// </summary>
    public interface IAnalysis
    {
        void Execute();
        IResults Results { get; }
        IParameters Parameters { get; }
        int Decimals { get; set; }
    }
}
