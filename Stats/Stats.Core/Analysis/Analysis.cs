using System;
using System.Linq;
using Stats.Core.Results;
using Stats.Core.Data;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel.Composition.Primitives;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Stats.Core.Analysis.Interfaces;
using Stats.Core.Environment;

namespace Stats.Core.Analysis
{
    public abstract class Analysis<P, R>: IAnalysis<P, R>
        where P: IParameters
        where R: IResults
    {
        private R results;

        public virtual R Results
        {
            get
            {
                return this.results;
            }
            protected set
            {
                this.results = value;
                this.subItems.Clear();
                this.subItems.Add(value);
            }
        }

        public virtual P Parameters
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public abstract void Execute();

        //TODO: remove DataMatrix from Analysis and move to Paramaters where applicable
        public IDataMatrix DataMatrix
        {
            get;
            set;
        }

        public IEnumerable<Type> GetInterfaces<T>()
        {
            //TODO: make this look for generic types and T-value...
           
            //var exports =
            //    from export in env.InterfaceModules
            //    where export.Metadata.InterfaceType.IsSubclassOf(typeof(T))
            //    where this.GetType().IsSubclassOf(export.Metadata.ObjectType)
            //    select export;
            return new List<Type>();

        }

        private ObservableCollection<IResults> subItems;
        public IEnumerable<IProjectItem> SubItems
        {
            get
            {
                if (this.subItems == null)
                {
                    this.subItems = new ObservableCollection<IResults>();
                }
                return this.subItems;
            }
        }
    }

    
}
