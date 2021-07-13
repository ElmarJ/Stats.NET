using Stats.Core.Results;
using Stats.Core.Data;
namespace Stats.Core.Analysis
{
    // In C#4.0 both P and R will be marked as out to support covariance.
    public interface IAnalysis<P, R>: IAnalysis
        where P: IParameters
        where R: IResults
    {
        new R Results { get; }
        new P Parameters { get; }
    }
}
