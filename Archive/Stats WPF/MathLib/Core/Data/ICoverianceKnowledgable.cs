using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.Core.Data
{
    public interface ICoverianceKnowledgable: IDataMatrix
    {
        double Covariance(IVariable variable1, IVariable variable2);
        double SumOfProducts(IVariable variable1, IVariable variable2);
    }
}
