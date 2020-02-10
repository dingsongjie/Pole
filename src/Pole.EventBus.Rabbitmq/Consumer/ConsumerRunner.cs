using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Pole.Core.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pole.Core;
using Pole.Core.Serialization;

namespace Pole.EventBus.RabbitMQ
{
    public class ConsumerRunner
    {
        readonly IMpscChannel<BasicDeliverEventArgs> mpscChannel;
        readonly ISerializer serializer;
        public ConsumerRunner(
            IRabbitMQClient client,
            IServiceProvider provider,
            RabbitConsumer consumer,
            QueueInfo queue)
        {
            Client = client;
            Logger = provider.GetService<ILogger<ConsumerRunner>>();
            serializer = provider.GetService<ISerializer>();
            mpscChannel = provider.GetService<IMpscChannel<BasicDeliverEventArgs>>();
            mpscChannel.BindConsumer(BatchExecuter);
            Consumer = consumer;
            Queue = queue;
        }
        public ILogger<ConsumerRunner> Logger { get; }
        public IRabbitMQClient Client { get; }
        public RabbitConsumer Consumer { get; }
        public QueueInfo Queue { get; }
        public ModelWrapper Model { get; set; }
        public EventingBasicConsumer BasicConsumer { get; set; }
        public bool IsUnAvailable => !BasicConsumer.IsRunning || Model.Model.IsClosed;
        private bool isFirst = true;
        public Task Run()
        {
            Model = Client.PullChannel();
            mpscChannel.Config(Model.Connection.Options.CunsumerMaxBatchSize, Model.Connection.Options.CunsumerMaxMillisecondsInterval);
            if (isFirst)
            {
                isFirst = false;
                Model.Model.ExchangeDeclare(Consumer.EventBus.Exchange, "direct", true);
                Model.Model.ExchangeDeclare(Queue.Queue, "direct", true);
                Model.Model.ExchangeBind(Consumer.EventBus.Exchange, Queue.Queue, string.Empty);
                Model.Model.QueueDeclare(Queue.Queue, true, false, false, null);
                Model.Model.QueueBind(Queue.Queue, Queue.Queue, string.Empty);
            }
            Model.Model.BasicQos(0, Model.Connection.Options.CunsumerMaxBatchSize, false);
            BasicConsumer = new EventingBasicConsumer(Model.Model);
            BasicConsumer.Received += async (ch, ea) => await mpscChannel.WriteAsync(ea);
            BasicConsumer.ConsumerTag = Model.Model.BasicConsume(Queue.Queue, Consumer.Config.AutoAck, BasicConsumer);
            return Task.CompletedTask;
        }
        public Task HeathCheck()
        {
            if (IsUnAvailable)
            {
                Close();
                return Run();
            }
            else
                return Task.CompletedTask;
        }
        private async Task BatchExecuter(List<BasicDeliverEventArgs> list)
        {
            if (list.Count == 1)
            {
                await Process(list.First());
            }
            else
            {
                try
                {
                    await Consumer.Notice(list.Select(o => o.Body).ToList());
                    if (!Consumer.Config.AutoAck)
                    {
                        Model.Model.BasicAck(list.Max(o => o.DeliveryTag), true);
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception.InnerException ?? exception, $"An error occurred in {Consumer.EventBus.Exchange}-{Queue}");
                    if (Consumer.Config.Reenqueue)
                    {
                        await Task.Delay(1000);
                        foreach (var item in list)
                        {
                            Model.Model.BasicReject(item.DeliveryTag, true);
                        }
                    }
                }
            }
        }
        private async Task Process(BasicDeliverEventArgs ea)
        {
            try
            {
                await Consumer.Notice(ea.Body);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"An error occurred in {Queue.Queue}, routing path {Consumer.EventBus.Exchange}->{Queue.Queue}->{Queue.Queue}");
                await ProcessComsumerErrors(ea, exception);
            }
            if (!Consumer.Config.AutoAck)
            {
                Model.Model.BasicAck(ea.DeliveryTag, false);
            }
        }

        private async Task ProcessComsumerErrors(BasicDeliverEventArgs ea, Exception exception)
        {
            if (Consumer.Config.Reenqueue)
            {
                if (ea.BasicProperties.Headers.TryGetValue(Consts.ConsumerRetryTimesStr, out object retryTimesObj))
                {
                    var retryTimes = Convert.ToInt32(retryTimesObj);
                    if (retryTimes <= Consumer.Config.MaxReenqueueTimes)
                    {
                        retryTimes++;
                        ea.BasicProperties.Headers[Consts.ConsumerRetryTimesStr] = retryTimes;
                        ea.BasicProperties.Headers[Consts.ConsumerExceptionDetailsStr] = serializer.Serialize(exception, typeof(Exception));
                        await Task.Delay((int)Math.Pow(2, retryTimes) * 1000).ContinueWith((task) =>
                        {
                            Model.Model.BasicReject(ea.DeliveryTag, true);
                        });
                    }
                    else
                    {
                        var errorQueueName = $"{Queue.Queue}{Consumer.Config.ErrorQueueSuffix}";
                        var errorExchangeName = $"{Queue.Queue}{Consumer.Config.ErrorQueueSuffix}";
                        Model.Model.ExchangeDeclare(errorExchangeName, "direct", true);
                        Model.Model.QueueDeclare(errorQueueName, true, false, false, null);
                        Model.Model.QueueBind(errorQueueName, errorExchangeName, string.Empty);
                    }
                    if (!Consumer.Config.AutoAck)
                    {
                        Model.Model.BasicAck(ea.DeliveryTag, false);
                    }
                }
            }
        }

        public void Close()
        {
            Model?.Dispose();
        }
    }
}
