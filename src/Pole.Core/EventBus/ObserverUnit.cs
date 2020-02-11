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
        readonly ITypeFinder typeFinder;
        readonly IClusterClient clusterClient;
        Func<byte[], Task> eventHandler;
        Func<List<byte[]>, Task> batchEventHandler;
        protected ILogger Logger { get; private set; }
        public Type GrainType { get; }

        public ObserverUnit(IServiceProvider serviceProvider, Type grainType)
        {
            this.serviceProvider = serviceProvider;
            clusterClient = serviceProvider.GetService<IClusterClient>();
            serializer = serviceProvider.GetService<ISerializer>();
            typeFinder = serviceProvider.GetService<ITypeFinder>();
            Logger = serviceProvider.GetService<ILogger<ObserverUnit<PrimaryKey>>>();
            GrainType = grainType;
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
        public ObserverUnit<PrimaryKey> UnreliableObserver(
            Func<IServiceProvider,
            FullyEvent<PrimaryKey>, ValueTask> handler)
        {
            eventHandler = EventHandler;
            batchEventHandler = BatchEventHandler;
            return this;
            //内部函数
            Task EventHandler(byte[] bytes)
            {
                var (success, transport) = EventBytesTransport.FromBytes<PrimaryKey>(bytes);
                if (success)
                {
                    var data = serializer.Deserialize(transport.EventBytes, typeFinder.FindType(transport.EventTypeCode));
                    if (data is IEvent @event && transport.GrainId is PrimaryKey actorId)
                    {
                        var eventBase = EventBase.FromBytes(transport.BaseBytes);
                        var tellTask = handler(serviceProvider, new FullyEvent<PrimaryKey>
                        {
                            StateId = actorId,
                            Base = eventBase,
                            Event = @event
                        });
                        if (!tellTask.IsCompletedSuccessfully)
                            return tellTask.AsTask();
                    }
                }
                return Task.CompletedTask;
            }
            Task BatchEventHandler(List<byte[]> list)
            {
                var groups =
                    list.Select(b => EventBytesTransport.FromBytes<PrimaryKey>(b))
                    .Where(o => o.success)
                    .Select(o => o.transport)
                    .GroupBy(o => o.GrainId);
                return Task.WhenAll(groups.Select(async kv =>
                {
                    foreach (var transport in kv)
                    {
                        var data = serializer.Deserialize(transport.EventBytes, typeFinder.FindType(transport.EventTypeCode));
                        if (data is IEvent @event && transport.GrainId is PrimaryKey actorId)
                        {
                            var eventBase = EventBase.FromBytes(transport.BaseBytes);
                            var tellTask = handler(serviceProvider, new FullyEvent<PrimaryKey>
                            {
                                StateId = actorId,
                                Base = eventBase,
                                Event = @event
                            });
                            if (!tellTask.IsCompletedSuccessfully)
                                await tellTask;
                        }
                    }
                }));
            }
        }
        public void Observer(Type observerType)
        {
            if (!typeof(PoleEventHandlerBase).IsAssignableFrom(observerType))
                throw new NotSupportedException($"{observerType.FullName} must inheritance from PoleEventHandler");
            eventHandler = EventHandler;
            batchEventHandler = BatchEventHandler;
            //内部函数
            Task EventHandler(byte[] bytes)
            {
                var (success, actorId) = EventBytesTransport.GetActorId<PrimaryKey>(bytes);
                if (success)
                {
                    return GetObserver(observerType, actorId).Invoke(new Immutable<byte[]>(bytes));

                }
                else
                {
                    if (Logger.IsEnabled(LogLevel.Error))
                        Logger.LogError($"{nameof(EventBytesTransport.GetActorId)} failed");
                }
                return Task.CompletedTask;
            }
            Task BatchEventHandler(List<byte[]> list)
            {
                var groups = list.Select(bytes =>
                {
                    var (success, GrainId) = EventBytesTransport.GetActorId<PrimaryKey>(bytes);
                    if (!success)
                    {
                        if (Logger.IsEnabled(LogLevel.Error))
                            Logger.LogError($"{nameof(EventBytesTransport.GetActorId)} failed");
                    }
                    return (success, GrainId, bytes);
                }).Where(o => o.success).GroupBy(o => o.GrainId);
                return Task.WhenAll(groups.Select(kv =>
                {
                    var items = kv.Select(item => item.bytes).ToList();
                    return GetObserver(observerType, kv.Key).Invoke(new Immutable<List<byte[]>>(items));
                }));
            }
        }
        static readonly ConcurrentDictionary<Type, Func<IClusterClient, PrimaryKey, string, PoleEventHandlerBase>> _observerGeneratorDict = new ConcurrentDictionary<Type, Func<IClusterClient, PrimaryKey, string, PoleEventHandlerBase>>();
        private PoleEventHandlerBase GetObserver(Type ObserverType, PrimaryKey primaryKey)
        {
            var func = _observerGeneratorDict.GetOrAdd(ObserverType, key =>
            {
                var clientType = typeof(IClusterClient);
                var clientParams = Expression.Parameter(clientType, "client");
                var primaryKeyParams = Expression.Parameter(typeof(PrimaryKey), "primaryKey");
                var grainClassNamePrefixParams = Expression.Parameter(typeof(string), "grainClassNamePrefix");
                var method = typeof(ClusterClientExtensions).GetMethod("GetGrain", new Type[] { clientType, typeof(PrimaryKey), typeof(string) });
                var body = Expression.Call(method.MakeGenericMethod(ObserverType), clientParams, primaryKeyParams, grainClassNamePrefixParams);
                return Expression.Lambda<Func<IClusterClient, PrimaryKey, string, PoleEventHandlerBase>>(body, clientParams, primaryKeyParams, grainClassNamePrefixParams).Compile();
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
