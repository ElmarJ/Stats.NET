using Stats.Core.Results;
namespace Stats.Core.Analysis.Interfaces
{
    public abstract class SyntaxConnector<T> where T : IAnalysis<IParameters, IResults>
    {
        public abstract T ParseSyntax(string syntax);
    }
}

