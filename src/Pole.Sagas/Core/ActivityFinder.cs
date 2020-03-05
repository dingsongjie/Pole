using Microsoft.Extensions.Logging;
using Pole.Core.Utils;
using Pole.Sagas.Core.Abstraction;
using Pole.Sagas.Core.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pole.Sagas.Core
{
    class ActivityFinder : IActivityFinder
    {
        private readonly ConcurrentDictionary<string, Type> nameDict = new ConcurrentDictionary<string, Type>();
        private readonly ConcurrentDictionary<Type, string> typeDict = new ConcurrentDictionary<Type, string>();
        readonly ILogger<ActivityFinder> logger;
        public ActivityFinder(ILogger<ActivityFinder> logger)
        {
            this.logger = logger;
            var baseActivityType = typeof(IActivity<>);
            foreach (var assembly in AssemblyHelper.GetAssemblies(this.logger))
            {
                foreach (var type in assembly.GetTypes().Where(m => m.IsGenericType && m.GetGenericTypeDefinition() == baseActivityType && !m.IsAbstract))
                {
                    if (!type.FullName.EndsWith("Activity"))
                    {
                        throw new ActivityNameIrregularException(type);
                    }
                    var activityName = type.Name.Substring(0, type.Name.Length - "Activity".Length);
                    typeDict.TryAdd(type, activityName);

                    if (!nameDict.TryAdd(activityName, type))
                    {
                        throw new ActivityNameRepeatedException(activityName);
                    }
                }
            }
        }
        public Type FindType(string name)
        {
            if (nameDict.TryGetValue(name, out Type type))
            {
                return type;
            }
            throw new ActivityNotFoundByNameException(name);
        }

        public string GetName(Type type)
        {
            if (!typeDict.TryGetValue(type, out var value))
                throw new ActivityNotFoundByTypeException(type);
            return value;
        }
    }
}
