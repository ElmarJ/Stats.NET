using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Analysis;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using Stats.Core.Core.AddIns;

namespace Stats.Core.Environment
{
    public class Environment
    {
        private ModuleLoader modules = new ModuleLoader();

        public Environment()
        {
        }

        public ModuleLoader Modules
        {
            get { return modules; }
        }

        public Project Project { get; set; }

        public void Analyse(IAnalysis analysis)
        {
            analysis.Execute();
            Project.AnalysisHistoryCollection.Add(analysis);
        }
    }
}
