using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Stats.Core.AddIns
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AnalysisMetadataAttribute : Attribute
    {
        public int Priority { get; private set; }
        public string Name { get; private set; }
        public AnalysisMetadataAttribute(int priority, string name)
        {
            this.Priority = priority;
            this.Name = name;
        }
    }
}
