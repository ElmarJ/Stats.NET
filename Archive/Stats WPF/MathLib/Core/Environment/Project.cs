using System;
using MathLib.Core.Analysis;

namespace MathLib.Core.Environment
{
    [Serializable]
    public class Project
    {
        private AnalysisCollection analysisHistoryCollection = new AnalysisCollection();
        private DataSetCollection dataSetColelction = new DataSetCollection();

        public AnalysisCollection AnalysisHistoryCollection
        {
            get { return analysisHistoryCollection; }
        }

        public DataSetCollection DataSetCollection
        {
            get { return dataSetColelction; }
        }
    }
}
