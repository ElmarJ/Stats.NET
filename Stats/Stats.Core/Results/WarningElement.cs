using System;
using Stats.Core.Results.Presenters;

namespace Stats.Core.Results
{
    public class WarningElement: Element
    {
        public override T Render<T>(IPresenter<T> presenter)
        {
            return presenter.RenderWarningElement(this);
        }

        public string Text { get; set; }
    }
}
