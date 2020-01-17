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
            var callbackType = typeof(IReliableEventCallback);

            var callbackTypes = assemblies.SelectMany(m => m.GetTypes().Where(type => callbackType.IsAssignableFrom(type)));
            return callbackTypes.ToList(); ;
        }
    }
}
