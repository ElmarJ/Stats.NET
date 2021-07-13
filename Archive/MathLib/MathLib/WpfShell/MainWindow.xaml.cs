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
using System.Data;
using MathLib.Statistics.Analysis;

namespace WpfShell
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        MathLib.Statistics.DataSet dataSet = new MathLib.Statistics.DataSet();
        DataColumn var1;
        DataColumn var2;
        DataColumn var3;

        List<Results> resultCollection = new List<Results>();

        public Window1()
        {
            InitializeComponent();

            DataTable table = new DataTable();
            var1 = new DataColumn("Var1", typeof(double));
            var2 = new DataColumn("Var2", typeof(double));
            var3 = new DataColumn("Var3", typeof(double));
            table.Columns.Add(var1);
            table.Columns.Add(var2);
            table.Columns.Add(var3);
            table.Rows.Add(1, 2, 3);
            table.Rows.Add(-1, 2, 4);
            table.Rows.Add(5, 2, 3);
            table.Rows.Add(8, -4, 6);
            table.Rows.Add(3, 5, 0);

            dataSet = new MathLib.Statistics.DataSet(table);

            this.dataGrid.DataContext = table;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MultipleLinearRegressionAnalysis regr;
            regr = new MultipleLinearRegressionAnalysis(this.dataSet.Variables[0], this.dataSet.Variables[1], this.dataSet.Variables[2]);
            regr.Execute();
            RegressionResults results = regr.Results;
            resultCollection.Add(results);


            resultFrame.Navigate("about:blank");
            resultFrame.Content = results.ToHtml();
            dataGrid.Visibility = Visibility.Hidden;
            resultFrame.Visibility = Visibility.Visible;
        }


    }
}
