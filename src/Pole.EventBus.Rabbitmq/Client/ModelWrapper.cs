using Microsoft.Extensions.ObjectPool;
using Pole.Core;
using Pole.Core.Exceptions;
using RabbitMQ.Client;
using System;

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
            persistentProperties.Headers.Add(Consts.ConsumerRetryTimesStr, consumeRetryTimesStr);
            noPersistentProperties = Model.CreateBasicProperties();
            noPersistentProperties.Persistent = false;
            noPersistentProperties.Headers.Add(Consts.ConsumerRetryTimesStr, consumeRetryTimesStr);
        }
        public void Publish(byte[] msg, string exchange, string routingKey, bool persistent = true)
        {
            Model.ConfirmSelect();
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
