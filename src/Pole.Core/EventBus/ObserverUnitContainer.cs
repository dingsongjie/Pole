using Microsoft.Extensions.Logging;
using Pole.Core.Observer;
using Pole.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pole.Core.EventBus.EventHandler;
using System.Linq;
using Pole.Core.Exceptions;

namespace Pole.Core.EventBus
{
    public class ObserverUnitContainer : IObserverUnitContainer
    {
        readonly ConcurrentDictionary<string, List<object>> unitDict = new ConcurrentDictionary<string, List<object>>();
        public ObserverUnitContainer(IServiceProvider serviceProvider)
        {
            var eventHandlerList = new List<(Type, EventHandlerAttribute)>();
            foreach (var assembly in AssemblyHelper.GetAssemblies(serviceProvider.GetService<ILogger<ObserverUnitContainer>>()))
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var attribute in type.GetCustomAttributes(false))
                    {
                        if (attribute is EventHandlerAttribute eventHandlerAttribute)
                        {
                            eventHandlerList.Add((type, eventHandlerAttribute));
                            break;
                        }
                    }
                }
            }
            foreach (var eventHandler in eventHandlerList)
            {
                var unitType = typeof(ObserverUnit<>).MakeGenericType(new Type[] { typeof(string) });
                var unit = (ObserverUnit<string>)Activator.CreateInstance(unitType, serviceProvider, eventHandler.Item1);
                unit.Observer();
                Register<string>(eventHandler.Item2.EventName, unit);
            }
        }
        public List<IObserverUnit<PrimaryKey>> GetUnits<PrimaryKey>(string observerName)
        {
            if (unitDict.TryGetValue(observerName, out var units))
            {
                if (units is List<IObserverUnit<PrimaryKey>> result)
                {
                    return result;
                }
                else
                    throw new UnmatchObserverUnitException(observerName);
            }
            else
                throw new UnfindObserverUnitException(observerName);
        }
        public List<object> GetUnits(string observerName)
        {
            if (unitDict.TryGetValue(observerName, out var unit))
            {
                return unit;
            }
            else
                throw new UnfindObserverUnitException(observerName);
        }

        public void Register<PrimaryKey>(string observerName, IGrainID observerUnit)
        {
            if (unitDict.TryGetValue(observerName, out List<object> units))
            {
                units.Add(observerUnit);
            }
            if (!unitDict.TryAdd(observerName, new List<object> { observerUnit }))
            {
                throw new ObserverUnitRepeatedException(observerUnit.EventHandlerType.FullName);
            }
        }

    }
}
