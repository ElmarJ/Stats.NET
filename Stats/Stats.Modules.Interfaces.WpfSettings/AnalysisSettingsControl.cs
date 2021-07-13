using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Stats.Core.Analysis;
using Stats.Core.Results;

namespace Stats.Modules.Interfaces.Analysis.Wpf
{
    // Todo: make this a generic interface that can be implemented by any control.

//    public class AnalysisSettingsControl<T>: UserControl where T: IAnalysis
    public class AnalysisSettingsControl: UserControl
    {
        public AnalysisSettingsControl()
            : base()
        {
        }

        public virtual AnalysisCollection Analysis
        {
            get;
            set;
        }

    }
}
