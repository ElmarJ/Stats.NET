using System;
using Stats.Core.Analysis;
using Stats.Core.Data;
using System.Collections;
using System.Collections.ObjectModel;

namespace Stats.Core.Environment
{
    [Serializable]
    public class Project: ProjectItemCollection<IProjectItem>
    {
        private AnalysisCollection analysisHistoryCollection = new AnalysisCollection();
        private DataSetCollection dataSetCollection = new DataSetCollection();

        public Project(): base()
        {
            this.Items.Add(analysisHistoryCollection);
            this.Items.Add(dataSetCollection);
        }

        public AnalysisCollection AnalysisHistoryCollection
        {
            get { return analysisHistoryCollection; }
        }

        public DataSetCollection DataSetCollection
        {
            get { return dataSetCollection; }
        }

        public DataMatrix MainDataMatrix
        {
            get;
            set;
        }

        public override string Name
        {
            get { return "A Project"; }
        }
    }
}
