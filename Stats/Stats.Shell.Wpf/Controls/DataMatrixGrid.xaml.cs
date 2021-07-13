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
using Stats.Core.Data;
using Microsoft.Windows.Controls;
using System.Collections.Specialized;
using Stats.Core.Data.Observations;

namespace Stats.Shells.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for DataMatrixGrid.xaml
    /// </summary>
    public partial class DataMatrixGrid : UserControl
    {
        public DataMatrixGrid()
        {
            InitializeComponent();
            AttachEventHandler();
        }

        private void AttachEventHandler()
        {
            INotifyCollectionChanged notifyCollection = this.DataContext as INotifyCollectionChanged;
            if (notifyCollection != null)
            {
                notifyCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(OnItemsSourceCollectionChanged);
            }
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CreateColumns();
        }


        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property.Name == "DataContext")
            {
                CreateColumns();
            }
        }
        private void CreateColumns()
        {

            DataMatrix matrix = this.DataContext as DataMatrix;

            this.DataGrid.Columns.Clear();

            foreach (IVariable<IObservation> variable in matrix.Variables)
            {
                string bindingPath = ".[" + variable.Name + "].Value";

                Binding newBinding = new Binding(bindingPath);

                // newBinding.Mode = BindingMode.OneWay;
                newBinding.StringFormat = "F";
                var column = new DataGridTextColumn { Header = variable.Name, Binding=newBinding };
                
                this.DataGrid.Columns.Add(column);
            }
        }
    }
}
