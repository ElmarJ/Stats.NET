using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MathLib.Core.Data
{
    public interface IVariable
    {
        Observation NewObservation(Record record);
        DataMatrix DataMatrix { get; }
        string Name { get; }
        Descriptives Descriptives { get; }
    }
}
