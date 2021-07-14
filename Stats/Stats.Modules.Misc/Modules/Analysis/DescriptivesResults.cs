using System;
using MathLib.Core.Analysis;
using MathLib.Core.Results;
using System.ComponentModel.Composition;

namespace MathLib.Modules.Analysis
{
    [Export(typeof(IResults))]
    public class DescriptivesResults: Results
    {
        public override ElementCollection Elements
        {
            get { throw new NotImplementedException(); }
        }
    }
}
