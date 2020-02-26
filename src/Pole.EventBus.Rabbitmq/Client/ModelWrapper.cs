using Microsoft.Extensions.ObjectPool;
using Pole.Core;
using Pole.Core.Exceptions;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Pole.EventBus.RabbitMQ
{
    public class ModelWrapper : IDisposable
    {
        readonly IBasicProperties persistentProperties;
        readonly IBasicProperties noPersistentProperties;
        public DefaultObjectPool<ModelWrapper> Pool { get; set; }
        public ConnectionWrapper Connection { get; set; }
        public IModel Model { get; set; }
        public ModelWrapper(
            ConnectionWrapper connectionWrapper,
            IModel model)
        {
            Connection = connectionWrapper;
            Model = model;
            var consumeRetryTimes = 0;
            var consumeRetryTimesStr = consumeRetryTimes.ToString();
            persistentProperties = Model.CreateBasicProperties();
            persistentProperties.Persistent = true;
            persistentProperties.Headers = new Dictionary<string, object>();
            persistentProperties.Headers.Add(Consts.ConsumerRetryTimesStr, consumeRetryTimesStr);
            noPersistentProperties = Model.CreateBasicProperties();
            noPersistentProperties.Persistent = false;
            noPersistentProperties.Headers = new Dictionary<string, object>();
            noPersistentProperties.Headers.Add(Consts.ConsumerRetryTimesStr, consumeRetryTimesStr);
        }
        public void Publish(byte[] msg, IDictionary<string, object> headers, string exchange, string routingKey, bool persistent = true)
        {
            if (persistent)
            {
                persistentProperties.Headers = headers;
            }
            else
            {
                noPersistentProperties.Headers = headers;
            }
            Model.BasicPublish(exchange, routingKey, persistent ? persistentProperties : noPersistentProperties, msg);
            if (!Model.WaitForConfirms(TimeSpan.FromSeconds(Connection.Options.ProducerConfirmWaitTimeoutSeconds), out bool isTimeout))
            {
                if (isTimeout)
                {
                    throw new ProducerConfirmTimeOutException(Connection.Options.ProducerConfirmWaitTimeoutSeconds);
                }
                else
                {
                    throw new ProducerReceivedNAckException();
                }
            }

        }
        public void WaitForConfirmsOrDie(TimeSpan timeSpan)
        {
            Model.WaitForConfirmsOrDie(timeSpan);
        }
        public void Publish(byte[] msg, string exchange, string routingKey, bool persistent = true)
        {
            Model.BasicPublish(exchange, routingKey, persistent ? persistentProperties : noPersistentProperties, msg);
        }
        public void Dispose()
        {
            Pool.Return(this);
        }
        public void ForceDispose()
        {
            Model.Close();
            Model.Dispose();
            Connection.Return(this);
        }
    }
}
