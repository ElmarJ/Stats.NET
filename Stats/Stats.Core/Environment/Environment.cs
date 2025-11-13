using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Analysis;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using Stats.Core.AddIns;
using Stats.Core.Results;
using System.IO;
using System.Reflection;
using Stats.Core.Analysis.Interfaces;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Stats.Core.Environment
{
    public sealed class Environment
    {

        [ImportMany(typeof(IAnalysis<IParameters, IResults>))]
        public ObservableCollection<IAnalysis<IParameters, IResults>> AnalysisModules { get; set; }

        //TODO: make this import interfaces
        [ImportMany(typeof(IInterface<IAnalysis<IParameters, IResults>>))]
        public List<IInterface<IAnalysis<IParameters, IResults>>> InterfaceModules { get; set; }

        public Environment()
        {
            //TODO: make this a singleton?
            Compose();
        }

        public Project Project { get; set; }

        public void Analyse(IAnalysis<IParameters, IResults> analysis)
        {
            analysis.Execute();
            Project.AnalysisHistoryCollection.Add(analysis);
        }

        private void Compose()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var catalog = new DirectoryCatalog(directoryName);
            var container = new CompositionContainer(catalog);

            container.SatisfyImportsOnce(this);
        }

        public static void SaveProject(Project project, string filename)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                // You may need to add custom converters if Project contains non-serializable types
            };
            var json = JsonSerializer.Serialize(project, options);
            File.WriteAllText(filename, json);
        }
    }
}
