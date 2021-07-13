using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Analysis;
using System.ComponentModel.Composition;
using Stats.Core.Results;

namespace Stats.Modules.Analysis.Functional
{
    [Export(typeof(IAnalysis<IParameters, IResults>))]
    public class AnalysisRepeater: Analysis<AnalysisRepeaterParameters, AnalysisRepeaterResults>
    {
        public override void Execute()
        {
            //Todo: implement run Analysis for all variables (adjust change parameter accordingly)
            throw new NotImplementedException();
        }
    }
}
