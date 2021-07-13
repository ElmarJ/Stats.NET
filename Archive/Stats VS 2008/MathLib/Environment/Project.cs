using System;
using Stats.Core.Analysis;
using Stats.Core.Data;

namespace Stats.Core.Environment
{
    [Serializable]
    public class Project
    {
        private AnalysisCollection analysisHistoryCollection = new AnalysisCollection();
        private DataSetCollection dataSetCollection = new DataSetCollection();

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
    }
}
