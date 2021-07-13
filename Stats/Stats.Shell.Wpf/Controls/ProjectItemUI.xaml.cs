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
using Stats.Core.Analysis;
using Stats.Core.Data;
using Stats.Core.Environment;
using Stats.Core.Results;


namespace Stats.Shells.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for ProjectElementUI.xaml
    /// </summary>
    public partial class ProjectItemUI : UserControl
    {

        public static readonly DependencyProperty ProjectItemProperty = 
            DependencyProperty.Register(
            "ProjectItem", typeof(IProjectItem),typeof(ProjectItemUI));

        public IProjectItem Item
        {
            get { return (IProjectItem)GetValue(ProjectItemProperty); }
            set
            {
                SetValue(ProjectItemProperty, value);
                this.SetInterface();
            }
        }

        private void SetInterface()
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        public ProjectItemUI()
        {
            InitializeComponent();
        }

        private void RunAnalysisCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        private void RunAnalysisExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var analysis = (IAnalysis<Parameters, Results>)e.Parameter;
            var myWindow = (MainWindow)((Grid)this.Parent).Parent;
            analysis.DataMatrix = myWindow.Environment.Project.MainDataMatrix;
            analysis.Execute();
        }

    }
}