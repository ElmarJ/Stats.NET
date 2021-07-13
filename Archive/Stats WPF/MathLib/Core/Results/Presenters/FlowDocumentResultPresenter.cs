﻿using System.Windows.Documents;
using MathLib.Core.Results;

namespace MathLib.Core.Analysis.Presenters
{
    public abstract class FlowDocumentResultPresenter<T>: IFlowDocumentResultPresenter<T>
        where T : IResults
    {
        private T results;

        public FlowDocumentResultPresenter(T results)
        {
            this.results = results;
        }

        protected T Results
        {
            get
            {
                return results;
            }
            
            set
            {
                results = value;
            }
        }

        T IFlowDocumentResultPresenter<T>.Results
        {
            set
            {
                this.Results = value;
            }
        }


        public abstract FlowDocument GenerateFlowDocument();
    }
}
