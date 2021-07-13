using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathLib.Core.Results.Presenters;

namespace MathLib.Core.Results
{
    public class TextElement: Element
    {
        public TextElement()
            : base()
        {
        }

        public TextElement(string text)
            : this()
        {
            this.Text = text;
        }

        public override T Render<T>(IPresenter<T> presenter)
        {
            return presenter.RenderTextElement(this);
        }

        public string Text
        {
            get;
            set;
        }
    }
}
