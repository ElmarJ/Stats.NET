using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathLib.Core.Results.Presenters;

namespace MathLib.Core.Results
{
    public interface IElement
    {
        T Render<T>(IPresenter<T> presenter);
    }
}
