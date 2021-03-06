using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Stats.Modules.Analysis;
using Stats.Interfaces.Wpf;

namespace Stats.Modules.Interfaces.Analysis.Wpf
{
    /// <summary>
    /// Interaction logic for LinearRegressionSettings.xaml
    /// </summary>
    public partial class LinearRegressionSettings : AnalysisSettingsControl
    {
        public LinearRegressionSettings()
        {
            InitializeComponent();
        }

        public override Stats.Core.Analysis.AnalysisCollection Analysis
        {
            get
            {
                var collection = new Core.Analysis.AnalysisCollection();
                var analysis = (LinearRegressionAnalysis)this.Resources["Analysis"];
                
                collection.Add(analysis);
                return collection;
            }
        }

    }
}
