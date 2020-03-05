using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core.Abstraction
{
    public interface IActivityFinder
    {
        Type FindType(string name);
        string GetName(Type type);
    }
}
