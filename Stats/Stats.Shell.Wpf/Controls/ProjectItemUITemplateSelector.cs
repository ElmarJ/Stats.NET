using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Stats.Core.Environment;
using Stats.Core.Data;
using Stats.Core.Results;
using Stats.Core.Analysis;

namespace Stats.Shells.Wpf.Controls
{
    public class ProjectItemUITemplateSelector: DataTemplateSelector
    {
        public DataTemplate DataMatrixTemplate { get; set; }
        public DataTemplate ResultsTemplate { get; set; }
        public DataTemplate AnalysisTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;
            IProjectItem projectItem = item as IProjectItem;

            if (projectItem is IDataMatrix)
            {
                return this.DataMatrixTemplate;
            }
            if (projectItem is IResults)
            {
                return this.ResultsTemplate;
            }
            if (projectItem is IAnalysis)
            {
                return this.AnalysisTemplate;
            }
            else
            {
                return this.DefaultTemplate;
            }
        }
    }
}
