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
using MathLib.Core.Analysis;
using MathLib.Core.Data;
using MathLib.Core.Environment;
using System.Data;
using MathLib.Modules.Analysis;
using MathLib.Core.Results;

namespace WpfShell
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        MathLib.Core.Data.DataMatrix dataSet = new MathLib.Core.Data.DataMatrix();

        public Window1()
        {
            InitializeComponent();

            dataSet = new MathLib.Core.Data.DataMatrix();

            dataSet.Variables.Add(new Variable { Name="Var1", DataMatrix=dataSet });
            dataSet.Variables.Add(new Variable { Name="Var2", DataMatrix=dataSet });
            dataSet.Variables.Add(new Variable { Name="Var3", DataMatrix=dataSet });

            dataSet.Add(new Record(dataSet, 1 , 2, 3));
            dataSet.Add(new Record(dataSet, -1 , 2, 4));
            dataSet.Add(new Record(dataSet, 5, 2, 3));
            dataSet.Add(new Record(dataSet, 8, -4, 6));
            dataSet.Add(new Record(dataSet, 3, 5, 0));

            this.DataContext = dataSet;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MultipleLinearRegressionAnalysis regr;
            regr = new MultipleLinearRegressionAnalysis(this.dataSet.Variables[0], this.dataSet.Variables[1]);
            regr.Execute();
            LinearRegressionResults results = regr.Results;
            ((Project)this.Resources["currentProject"]).AnalysisHistoryCollection.Add(regr);

            // resultFrame.Content = results;
            dataGrid.Visibility = Visibility.Hidden;
            resultFrame.Visibility = Visibility.Visible;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            CorrelationAnalysis regr;
            regr = new CorrelationAnalysis(this.dataSet.Variables[0], this.dataSet.Variables[1]);
            regr.Execute();
            Results results = regr.Results;
            ((Project)this.Resources["currentProject"]).AnalysisHistoryCollection.Add(regr);

            // resultFrame.Content = results;
            dataGrid.Visibility = Visibility.Hidden;
            resultFrame.Visibility = Visibility.Visible;
            
        }


    }
}
