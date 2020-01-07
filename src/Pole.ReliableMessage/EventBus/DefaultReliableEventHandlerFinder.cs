using Pole.ReliableMessage.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pole.ReliableMessage.EventBus
{
    public class DefaultReliableEventHandlerFinder : IReliableEventHandlerFinder
    {
        public List<Type> FindAll(IEnumerable<Assembly> assemblies)
        {
            var eventHandlerType = typeof(IReliableEventHandler);

            var eventHandlerTypes = assemblies.SelectMany(m => m.GetTypes().Where(type => eventHandlerType.IsAssignableFrom(type)));
            return eventHandlerTypes.ToList(); 
        }
    }
}
