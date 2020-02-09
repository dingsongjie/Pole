using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core.Abstraction
{
    public interface ITypeFinder
    {
        Type FindType(string code);
        string GetCode(Type type);
    }
}
