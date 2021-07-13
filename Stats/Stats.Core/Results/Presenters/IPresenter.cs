using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Results.Presenters
{
    // In C# 4.0 T as out to support covariance
    public interface IPresenter<T>
    {
        T RenderTableElement(TableElement element);

        T RenderTextElement(TextElement element);

        T RenderWarningElement(WarningElement element);

        T RenderResults(Results element);
    }
}
