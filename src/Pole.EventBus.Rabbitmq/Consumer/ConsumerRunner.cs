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
using Microsoft.Extensions.Options;
using System.Text;

namespace Pole.EventBus.RabbitMQ
{
    public class ConsumerRunner
    {
        readonly IMpscChannel<BasicDeliverEventArgs> mpscChannel;
        readonly ISerializer serializer;
        readonly RabbitOptions rabbitOptions;
        List<ulong> errorMessageDeliveryTags = new List<ulong>();
        public ConsumerRunner(
            IRabbitMQClient client,
            IServiceProvider provider,
            RabbitConsumer consumer,
            QueueInfo queue,
            RabbitOptions rabbitOptions)
        {
            Client = client;
            Logger = provider.GetService<ILogger<ConsumerRunner>>();
            serializer = provider.GetService<ISerializer>();
            mpscChannel = provider.GetService<IMpscChannel<BasicDeliverEventArgs>>();
            mpscChannel.BindConsumer(BatchExecuter);
            Consumer = consumer;
            Queue = queue;
            this.rabbitOptions = rabbitOptions;


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
                Model.Model.ExchangeDeclare($"{rabbitOptions.Prefix}{Consumer.EventBus.Exchange}", "direct", true);
                Model.Model.ExchangeDeclare(Queue.Queue, "direct", true);
                Model.Model.ExchangeBind(Queue.Queue, $"{rabbitOptions.Prefix}{Consumer.EventBus.Exchange}", string.Empty);
                Model.Model.QueueDeclare(Queue.Queue, true, false, false, null);
                Model.Model.QueueBind(Queue.Queue, Queue.Queue, string.Empty);
            }
            Model.Model.BasicQos(0, Model.Connection.Options.CunsumerMaxBatchSize, false);
            BasicConsumer = new EventingBasicConsumer(Model.Model);
            BasicConsumer.Received += async (ch, ea) =>
            {
                await mpscChannel.WriteAsync(ea);
            };
            BasicConsumer.ConsumerTag = Model.Model.BasicConsume(Queue.Queue, Consumer.Config.AutoAck, BasicConsumer);
            return Task.CompletedTask;
        }
        private async Task BatchExecuter(List<BasicDeliverEventArgs> list)
        {
            try
            {
                await Consumer.Notice(list.Select(o => o.Body).ToList());
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, $"An error occurred in batch consume {Queue.Queue} queue, routing path {Consumer.EventBus.Exchange}->{Queue.Queue}->{Queue.Queue}");
                if (Consumer.Config.Reenqueue)
                {
                    foreach (var item in list)
                    {
                        await ProcessComsumerErrors(item, exception);
                    }
                    return;
                }
            }
            if (!Consumer.Config.AutoAck)
            {
                if (errorMessageDeliveryTags.Count == 0)
                {
                    Model.Model.BasicAck(list.Max(o => o.DeliveryTag), true);
                }
                else
                {
                    list.ForEach(m =>
                    {
                        Model.Model.BasicAck(m.DeliveryTag, false);
                    });
                }
            }
        }

        private async Task ProcessComsumerErrors(BasicDeliverEventArgs ea, Exception exception)
        {
            // todo 这里需要添加断路器 防止超量的 Task.Delay

            if (ea.BasicProperties.Headers.TryGetValue(Consts.ConsumerRetryTimesStr, out object retryTimesObj))
            {
                errorMessageDeliveryTags.Add(ea.DeliveryTag);

                var retryTimesStr = Encoding.UTF8.GetString((byte[])retryTimesObj);
                var retryTimes = Convert.ToInt32(retryTimesStr);
                if (retryTimes < Consumer.Config.MaxReenqueueTimes)
                {
                    retryTimes++;
                    ea.BasicProperties.Headers[Consts.ConsumerRetryTimesStr] = retryTimes.ToString();
                    ea.BasicProperties.Headers[Consts.ConsumerExceptionDetailsStr] = exception.InnerException != null ? exception.InnerException.Message + exception.StackTrace : exception.Message + exception.StackTrace;
                    await Task.Delay((int)Math.Pow(2, retryTimes) * 1000).ContinueWith((task) =>
                    {
                        using var channel = Client.PullChannel();
                        channel.Publish(ea.Body, ea.BasicProperties.Headers, Queue.Queue, string.Empty, true);
                        Model.Model.BasicAck(ea.DeliveryTag, false);

                        errorMessageDeliveryTags.Remove(ea.DeliveryTag);
                    });
                }
                else
                {
                    var errorQueueName = $"{Queue.Queue}{Consumer.Config.ErrorQueueSuffix}";
                    var errorExchangeName = $"{Queue.Queue}{Consumer.Config.ErrorQueueSuffix}";
                    Model.Model.ExchangeDeclare(errorExchangeName, "direct", true);
                    Model.Model.QueueDeclare(errorQueueName, true, false, false, null);
                    Model.Model.QueueBind(errorQueueName, errorExchangeName, string.Empty);
                    using var channel = Client.PullChannel();
                    channel.Publish(ea.Body, ea.BasicProperties.Headers, errorExchangeName, string.Empty, true);
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
