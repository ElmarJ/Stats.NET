using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stats.Core.Analysis.Interfaces
{
    public interface IInterfaceMetadata
    {
        Type ObjectType { get; }
        Type InterfaceType { get; }
    }
}
