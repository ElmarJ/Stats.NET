using System;
using MathLib.Core.Analysis;
using MathLib.Core.Results;
using System.ComponentModel.Composition;

namespace MathLib.Modules.Analysis
{
    [Analysis("Produces Descriptives", 1)]
    public class DescriptivesAnalysis : Analysis<ParametersPlaceholder, DescriptivesResults>
    {

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
