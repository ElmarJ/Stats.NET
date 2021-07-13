namespace Stats.Core.Analysis.Interfaces
{
    public abstract class SyntaxConnector<T> where T : IAnalysis
    {
        public abstract T ParseSyntax(string syntax);
    }
}

