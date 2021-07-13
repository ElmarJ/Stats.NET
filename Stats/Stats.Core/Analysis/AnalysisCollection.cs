using System.Collections.ObjectModel;
using Stats.Core.Results;
using System;
using Stats.Core.Environment;

namespace Stats.Core.Analysis
{
    [Serializable]
    public class AnalysisCollection : ProjectItemCollection<IAnalysis<IParameters, IResults>>
    {
        public override string Name
        {
            get { return "All your Analysis"; }
        }
    }
}
