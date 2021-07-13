using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Core.Environment;
using Stats.Core.Results.Presenters;

namespace Stats.Core.Results
{
    public interface IElement: IProjectItem
    {
        T Render<T>(IPresenter<T> presenter);
    }
}
