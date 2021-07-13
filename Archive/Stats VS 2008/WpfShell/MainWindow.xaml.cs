using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Stats.Core.Analysis;
using Stats.Core.Data;
using Stats.Core.Environment;
using System.Data;
using Stats.Modules.Analysis;
using Stats.Core.Results;
using Stats.Core.Data.Observations;
using Stats.Core.Data.Variables;

namespace Stats.Shells.Wpf
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Stats.Core.Data.DataMatrix dataSet = new Stats.Core.Data.DataMatrix();
        Stats.Core.Environment.Environment env;
        Stats.Core.Environment.Project project;

        public MainWindow()
        {
            InitializeComponent();

            this.project = ((Project)this.Resources["currentProject"]);
            this.env = ((Stats.Core.Environment.Environment)this.Resources["currentEnvironment"]);
            this.env.Project = project;

            project.DataSetCollection.Add(dataSet);
            project.MainDataMatrix = dataSet;

            dataSet.Variables.Add<NummericalVariable>("var1");
            dataSet.Variables.Add<NummericalVariable>("var2");
            dataSet.Variables.Add<NummericalVariable>("var3");

            dataSet.Records.Add(new Record(dataSet, 1, 2, 3));
            dataSet.Records.Add(new Record(dataSet, 3, 2, 1));
            dataSet.Records.Add(new Record(dataSet, 6, 3, 7));
            dataSet.Records.Add(new Record(dataSet, 4, 7, 4));
            dataSet.Records.Add(new Record(dataSet, 3, 2, 8));
            dataSet.Records.Add(new Record(dataSet, 7, 5, 6));

            this.DataContext = dataSet;

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            LinearRegressionAnalysis regr;
            regr = new LinearRegressionAnalysis(this.dataSet.Variables[0], this.dataSet.Variables[1]);
            regr.Execute();
            LinearRegressionResults results = regr.Results;
            ((Project)this.Resources["currentProject"]).AnalysisHistoryCollection.Add(regr);

            resultFrame.Visibility = Visibility.Visible;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            CorrelationAnalysis regr;
            regr = new CorrelationAnalysis(this.dataSet.Variables[0], this.dataSet.Variables[1], this.dataSet.Variables[2]);
            regr.Execute();
            Results results = regr.Results;
            ((Project)this.Resources["currentProject"]).AnalysisHistoryCollection.Add(regr);

            resultFrame.Visibility = Visibility.Visible;
            
        }

    }
}
