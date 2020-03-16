using Microsoft.Extensions.Logging;
using Pole.Core.Domain;
using Pole.Core.Utils;
using Pole.Core.Utils.Abstraction;
using Pole.EventBus.Abstraction;
using Pole.EventBus.Event;
using Pole.EventBus.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pole.EventBus
{
    public class EventTypeFinder : IEventTypeFinder
    {
        private readonly ConcurrentDictionary<string, Type> codeDict = new ConcurrentDictionary<string, Type>();
        private readonly ConcurrentDictionary<Type, string> typeDict = new ConcurrentDictionary<Type, string>();
        readonly ILogger<EventTypeFinder> logger;
        public EventTypeFinder(ILogger<EventTypeFinder> logger)
        {
            this.logger = logger;
            var baseEventType = typeof(IEvent);
            foreach (var assembly in AssemblyHelper.GetAssemblies(this.logger))
            {
                foreach (var type in assembly.GetTypes().Where(m => baseEventType.IsAssignableFrom(m)&&!m.IsAbstract))
                {
                    var eventCode = type.FullName;
                    var eventAttribute = type.GetCustomAttributes(typeof(EventInfoAttribute),false).FirstOrDefault();
                    if (eventAttribute is EventInfoAttribute attribute )
                    {
                        eventCode = attribute.EventName;
                    }
                    typeDict.TryAdd(type, eventCode);

                    if (!codeDict.TryAdd(eventCode, type))
                    {
                        throw new TypeCodeRepeatedException(type.FullName, type.FullName);
                    }
                }
            }
        }
        /// <summary>
        /// 通过code获取Type对象
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public Type FindType(string typeCode)
        {
            if (codeDict.TryGetValue(typeCode, out Type type))
            {
                return type;
            }
            throw new UnknowTypeCodeException(typeCode);
        }
        /// <summary>
        /// 获取Type对象的code字符串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetCode(Type type)
        {
            if (!typeDict.TryGetValue(type, out var value))
                return type.FullName;
            return value;
        }
    }
}
