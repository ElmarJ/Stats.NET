using Stats.Core.Results;
using Stats.Core.Data;
using System.Collections.Generic;
using System;
using Stats.Core.Environment;
namespace Stats.Core.Analysis
{
    public interface IAnalysis : IProjectItemContainer
    {
        string Name { get; set; }
        void Execute();
        IEnumerable<Type> GetInterfaces<T>();
    }

    public interface IAnalysis<out P, out R> : IAnalysis
        where P: IParameters
        where R: IResults
    {
        R Results { get; }
        P Parameters { get; }
        IDataMatrix DataMatrix { get; set; }
    }
}
