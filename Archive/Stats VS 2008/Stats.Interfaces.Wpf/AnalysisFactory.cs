using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Stats.Core.Analysis;

namespace Stats.Interfaces.Wpf
{
    public class AnalysisSetupControl: UserControl, IAnalysisFactory
    {
        public AnalysisSetupControl()
            : base()
        {
        }

        IAnalysis IAnalysisFactory.Analysis
        {
            get { throw new NotImplementedException(); }
        }
    }
}
