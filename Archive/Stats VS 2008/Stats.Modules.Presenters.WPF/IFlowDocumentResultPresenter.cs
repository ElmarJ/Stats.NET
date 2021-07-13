using System.Windows.Documents;
using Stats.Core.Results;

namespace Stats.Core.Analysis.Presenters
{
    // In C#4.0: T as in to support contravariance
    interface IFlowDocumentResultPresenter<T> where T : IResults
    {
        T Results { set; }

        FlowDocument GenerateFlowDocument();
    }
}
