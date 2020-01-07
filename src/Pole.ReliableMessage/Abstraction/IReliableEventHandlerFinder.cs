using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IReliableEventHandlerFinder
    {
        List<Type> FindAll(IEnumerable<Assembly> assemblies);
    }
}
