using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Pole.EventBus.RabbitMQ
{
    public class ConnectionWrapper
    {
        private readonly List<ModelWrapper> channels = new List<ModelWrapper>();
        private readonly IConnection connection;
        readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
        public ConnectionWrapper(
            IConnection connection,
           RabbitOptions options)
        {
            this.connection = connection;
            Options = options;
        }
        public RabbitOptions Options { get; }
        public (bool success, ModelWrapper model) Get()
        {
            semaphoreSlim.Wait();
            try
            {
                if (channels.Count < Options.MasChannelsPerConnection)
                {
                    var channel = new ModelWrapper(this, connection.CreateModel());
                    channels.Add(channel);
                    return (true, channel);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
            return (false, default);
        }
        public void Return(ModelWrapper model)
        {
            channels.Remove(model);
        }
    }
}
