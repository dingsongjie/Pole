using Pole.ReliableMessage.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pole.ReliableMessage.EventBus
{
    class DefaultReliableEventCallBackFinder : IReliableEventCallBackFinder
    {
        public List<Type> FindAll(IEnumerable<Assembly> assemblies)
        {
            var eventType = typeof(IReliableEventCallback);

            var eventTypes = assemblies.SelectMany(m => m.GetTypes().Where(type => eventType.IsAssignableFrom(type)));
            return eventTypes.ToList(); ;
        }
    }
}
