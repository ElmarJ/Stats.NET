using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathLib.Core.Analysis;

namespace MathLib.Core.Environment
{
    public class Environment
    {
        public Project Project { get; set; }

        public void Analyse(Core.Analysis.IAnalysis analysis)
        {
            analysis.Execute();
            Project.AnalysisHistoryCollection.Add(analysis);
        }

        private void BuildAddinLists()
        {
        }
    }
}
