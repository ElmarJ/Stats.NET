using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Results.Presenters
{
    public abstract class Presenter<T>: IPresenter<T>
    {
        public abstract T RenderTableElement(TableElement element);

        public abstract T RenderTextElement(TextElement element);

        public abstract T RenderWarningElement(WarningElement element);

        public abstract T RenderResults(Results element);
    }
}
