using MathLib.Core.Results;
using MathLib.Core.Data;
namespace MathLib.Core.Analysis
{
    // In C#4.0 both P and R will be marked as out to support covariance.
    public interface IAnalysis<P, R>: IAnalysis
        where P: IParameters
        where R: IResults
    {
        new R Results { get; }
        new P Parameters { get; }
        void Execute(ICalculator<P, R> calculator);
    }
}
