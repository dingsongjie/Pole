using Microsoft.Extensions.Logging;
using Pole.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pole.EventBus.EventHandler;
using System.Linq;
using Pole.Core.Exceptions;
using Pole.EventBus.Event;

namespace Pole.EventBus
{
    public class ObserverUnitContainer : IObserverUnitContainer
    {
        readonly ConcurrentDictionary<string, List<object>> unitDict = new ConcurrentDictionary<string, List<object>>();
        public ObserverUnitContainer(IServiceProvider serviceProvider)
        {
            var eventHandlerList = new List<(Type, EventInfoAttribute)>();
            foreach (var assembly in AssemblyHelper.GetAssemblies(serviceProvider.GetService<ILogger<ObserverUnitContainer>>()))
            {
                foreach (var type in assembly.GetTypes().Where(m => typeof(IPoleEventHandler).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract && !typeof(Orleans.Runtime.GrainReference).IsAssignableFrom(m)))
                {
                    var eventType = type.BaseType.GetGenericArguments().FirstOrDefault();
                    var attribute = eventType.GetCustomAttributes(typeof(EventInfoAttribute), false).FirstOrDefault();

                    if (attribute != null)
                    {
                        eventHandlerList.Add((type, (EventInfoAttribute)attribute));
                    }
                    else
                    {
                        throw new PoleEventHandlerImplementException("Can not found EventHandlerAttribute in PoleEventHandler");
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
                return units.Select(m => (IObserverUnit<PrimaryKey>)m).ToList();
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
