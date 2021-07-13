using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Stats.Core.Analysis;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Stats.Core.AddIns;

namespace Stats.Core.Core.AddIns
{
    public class ModuleLoader
    {
        private DirectoryCatalog catalog;
        private CompositionContainer container;

        public ModuleLoader()
        {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            catalog = new DirectoryCatalog(directoryName);
            container = new CompositionContainer(catalog);

            container.SatisfyImports(this);
        }

        public void Refresh()
        {
            this.catalog.Refresh();
        }

        [Import]
        public ExportCollection<IAnalysis, IAnalysisMetadata> AnalysisModules { get; set; }
    }
}
