using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.EventBus
{
    public interface IEventTypeFinder
    {
        Type FindType(string code);
        string GetCode(Type type);
    }
}
