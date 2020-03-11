using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Client.Abstraction
{
    public interface IActivityFinder
    {
        Type FindType(string name);
        string GetName(Type type);
    }
}
