using Stats.Core.Results;
using Stats.Core.Data;
using System.ComponentModel.Composition;
namespace Stats.Core.Analysis
{

    /// <summary>
    /// Defines methods
    /// </summary>
    [ContractType(MetadataViewType=typeof(IAnalysisMetadata))]
    public interface IAnalysis
    {
        void Execute();
        IResults Results { get; }
        IParameters Parameters { get; }
        int Decimals { get; set; }
        IDataMatrix DataMatrix { get; set; }
    }
}
