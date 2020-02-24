using Microsoft.Extensions.Logging;
using Orleans;
using Pole.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pole.Core.Abstraction;
using System.Linq;
using Pole.Core.EventBus.Event;
using Orleans.Concurrency;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Pole.Core.EventBus.EventHandler;

namespace Pole.Core.EventBus
{
    public class ObserverUnit<PrimaryKey> : IObserverUnit<PrimaryKey>
    {
        readonly IServiceProvider serviceProvider;
        readonly ISerializer serializer;
        readonly IEventTypeFinder typeFinder;
        readonly IClusterClient clusterClient;
        Func<byte[], Task> eventHandler;
        Func<List<byte[]>, Task> batchEventHandler;
        protected ILogger Logger { get; private set; }
        public Type EventHandlerType { get; }

        public ObserverUnit(IServiceProvider serviceProvider, Type eventHandlerType)
        {
            this.serviceProvider = serviceProvider;
            clusterClient = serviceProvider.GetService<IClusterClient>();
            serializer = serviceProvider.GetService<ISerializer>();
            typeFinder = serviceProvider.GetService<IEventTypeFinder>();
            Logger = serviceProvider.GetService<ILogger<ObserverUnit<PrimaryKey>>>();
            EventHandlerType = eventHandlerType;
        }
        public static ObserverUnit<PrimaryKey> From<Grain>(IServiceProvider serviceProvider) where Grain : Orleans.Grain
        {
            return new ObserverUnit<PrimaryKey>(serviceProvider, typeof(Grain));
        }

        public Func<byte[], Task> GetEventHandler()
        {
            return eventHandler;
        }

        public Func<List<byte[]>, Task> GetBatchEventHandler()
        {
            return batchEventHandler;
        }

        public void Observer()
        {
            if (!typeof(IPoleEventHandler).IsAssignableFrom(EventHandlerType))
                throw new NotSupportedException($"{EventHandlerType.FullName} must inheritance from PoleEventHandler");
            eventHandler = EventHandler;
            batchEventHandler = BatchEventHandler;
            //内部函数
            Task EventHandler(byte[] bytes)
            {
                var (success, transport) = EventBytesTransport.FromBytes(bytes);
                if (success)
                {
                    return GetObserver(EventHandlerType, transport.EventId).Invoke(transport);
                }
                else
                {
                    if (Logger.IsEnabled(LogLevel.Error))
                        Logger.LogError($" EventId:{nameof(EventBytesTransport.EventId)} is not a event");
                }
                return Task.CompletedTask;
            }
            Task BatchEventHandler(List<byte[]> list)
            {
                var transports = list.Select(bytes =>
                {
                    var (success, transport) = EventBytesTransport.FromBytes(bytes);
                    if (!success)
                    {
                        if (Logger.IsEnabled(LogLevel.Error))
                            Logger.LogError($" EventId:{nameof(EventBytesTransport.EventId)} is not a event");
                    }
                    return (success, transport);
                }).Where(o => o.success)
                  .Select(o => (o.transport))
                  .ToList();
                // 批量处理的时候 grain Id 取第一个 event的id
                return GetObserver(EventHandlerType, transports.First().EventId).Invoke(transports);
            }
        }
        static readonly ConcurrentDictionary<Type, Func<IClusterClient, string, string, IPoleEventHandler>> _observerGeneratorDict = new ConcurrentDictionary<Type, Func<IClusterClient, string, string, IPoleEventHandler>>();
        private IPoleEventHandler GetObserver(Type ObserverType, string primaryKey)
        {
            var func = _observerGeneratorDict.GetOrAdd(ObserverType, key =>
            {
                var clientType = typeof(IClusterClient);
                var clientParams = Expression.Parameter(clientType, "client");
                var primaryKeyParams = Expression.Parameter(typeof(string), "primaryKey");
                var grainClassNamePrefixParams = Expression.Parameter(typeof(string), "grainClassNamePrefix");
                var method = typeof(ClusterClientExtensions).GetMethod("GetGrain", new Type[] { clientType, typeof(string), typeof(string) });
                var body = Expression.Call(method.MakeGenericMethod(ObserverType), clientParams, primaryKeyParams, grainClassNamePrefixParams);
                return Expression.Lambda<Func<IClusterClient, string, string, IPoleEventHandler>>(body, clientParams, primaryKeyParams, grainClassNamePrefixParams).Compile();
            });
            return func(clusterClient, primaryKey, null);
        }
    }
    public static class ClusterClientExtensions
    {
        public static TGrainInterface GetGrain<TGrainInterface>(IClusterClient client, string primaryKey, string grainClassNamePrefix = null) where TGrainInterface : IGrainWithStringKey
        {
            return client.GetGrain<TGrainInterface>(primaryKey, grainClassNamePrefix);
        }
    }
}
