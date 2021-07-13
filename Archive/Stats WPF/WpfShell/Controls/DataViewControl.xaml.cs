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

using System.Reflection;
using System.Collections.Specialized;
using MathLib.Core.Data;

namespace WpfShell.Controls
{
    /// <summary>
    /// Interaction logic for AutoGridListView.xaml
    /// </summary>
    public partial class DataViewControl : UserControl
    {
        Type dataType;

        public DataViewControl()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (dataType == null)
            {
                dataType = (sender as Grid).DataContext.GetType();
                SetListView();
            }
        }

        private void SetListView()
        {
            DataMatrix dataMatrix = this.DataContext as DataMatrix;

            ListView lv = new ListView();
            Binding binding = new Binding();
            binding.Source = dataMatrix;
            lv.SetBinding(ListView.ItemsSourceProperty, binding);

            GridView gv = new GridView();

            var colNames =
                from v in dataMatrix.Variables
                select v.Name;

            foreach (string colName in colNames)
            {
                GridViewColumn col = new GridViewColumn();
                col.Header = colName;
                col.DisplayMemberBinding = new Binding(".[" + colName + "].NummericalRepresentation");
                gv.Columns.Add(col);
            }

            lv.View = gv;
            grid.Children.Add(lv);
        }

        private StringCollection GetProperties()
        {
            StringCollection properties = new StringCollection();
            foreach (PropertyInfo property in dataType.GetProperties())
                properties.Add(property.Name);
            return properties;
        }
    }
}