using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Analysis;
using Stats.Core.Data;
using Stats.Core.Results;

namespace Stats.Modules.Analysis.Functional
{
    public class AnalysisRepeaterParameters: IParameters
    {
        public IAnalysis<IParameters, IResults> Analysis { get; set; }
        public IParameters DefaultParameters { get; set; }
        public VariableCollection Variables { get; set; }
        public string PropertyName { get; set; }
    }
}
