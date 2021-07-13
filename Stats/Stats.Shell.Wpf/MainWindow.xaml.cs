using System.Windows;
using Stats.Core.Data;
using Stats.Core.Environment;
using Stats.Core.Results;
using Stats.Core.Data.Variables;
using System.Windows.Input;
using Stats.Core.Analysis;

namespace Stats.Shells.Wpf
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Environment Environment { get; set; }

        public MainWindow()
        {
            this.Environment = new Core.Environment.Environment();
            this.Environment.Project = new Project();

            InitializeComponent();
        }

        private void AddNewAnalysisCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void AddNewAnalysisExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var analysis = e.Parameter as IAnalysis<IParameters, IResults>;
            analysis.Name = "Analyse 1";
            Environment.Project.AnalysisHistoryCollection.Add(analysis);
        }

        private void OpenSpssFile()
        {
            Stats.Core.Data.DataMatrix dataMatrix;

            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "SPSS File|*.sav";
            dlg.ShowDialog();
            using (var stream = dlg.OpenFile())
            {
                var reader = new SavFileLibrary.FileParser.SavFileParser(stream);
                var spssDataset = new SavFileLibrary.SpssDataset.SpssDataset(reader);
                dataMatrix = Stats.Interoperability.SPSS.SpssDataLoader.FromSpssFile(spssDataset);
            }

            this.DataContext = dataMatrix;
            Environment.Project.DataSetCollection.Add(dataMatrix);
            Environment.Project.MainDataMatrix = dataMatrix;
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void OpenFile_Handler(object sender, ExecutedRoutedEventArgs e)
        {
            OpenSpssFile();
        }
    }
}
