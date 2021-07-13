using System;
using Stats.Core.Analysis;
using Stats.Core.Results;
using System.ComponentModel.Composition;

namespace Stats.Modules.Analysis
{
    [Analysis("Produces Descriptives", 1)]
    [Export(typeof(IAnalysis<IParameters, IResults>))]
    public class DescriptivesAnalysis : Analysis<ParametersPlaceholder, DescriptivesResults>
    {

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
