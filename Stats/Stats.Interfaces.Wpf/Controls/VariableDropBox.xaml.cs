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
using Stats.Core.Data.Observations;
using Stats.Core.Data;

namespace Stats.Interfaces.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class VariableDropBox : UserControl
    {
        public VariableDropBox()
        {
            InitializeComponent();
        }

        private void variableListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(IVariable<IObservation>)))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void variableListBox_Drop(object sender, DragEventArgs e)
        {
            var variables = this.DataContext as VariableCollection;
            variables.Add(e.Data.GetData(typeof(IVariable<IObservation>)) as IVariable<IObservation>);
        }
    }
}
