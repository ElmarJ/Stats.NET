using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Stats.Core.Analysis
{
    [MetadataAttribute]
    public sealed class AnalysisAttribute : Attribute, IAnalysisMetadata
    {
        public AnalysisAttribute(string description, int priority)
        {
            this.Description = description;
            this.Priority = priority;
        }

        public string Description { get; private set; }
        public int Priority { get; private set; }
    }
}
