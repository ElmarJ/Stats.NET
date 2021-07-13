using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Xml.Serialization;

namespace MathLib.Core.Data
{
    [Serializable]
    public class Variable<T> : Variable, IVariable
        where T : Observation
    {
        public T this[Record record]
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public T NewObservation()
        {
            throw new NotImplementedException();
        }

    }
}
