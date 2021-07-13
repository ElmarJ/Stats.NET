using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Results.Presenters;

namespace Stats.Core.Results
{
    public abstract class Element: IElement
    {
        public abstract T Render<T>(IPresenter<T> presenter);

        string Environment.IProjectItem.Name
        {
            get { return "Element Name"; }
        }
    }
}
