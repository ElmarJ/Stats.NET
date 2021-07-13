using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Results;

namespace Stats.Modules.Analysis.Functional
{
    public class AnalysisRepeaterResults: Results
    {
        private List<Results> results;

        internal AnalysisRepeaterResults(List<Results> results)
        {
            this.results = results;
        }

        public override ElementCollection Elements
        {
            get
            {
                IList<IElement> elements = (IList<IElement>)this.results;
                return new ElementCollection(elements);
            }
        }
    }
}
