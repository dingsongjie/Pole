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
        readonly IChannel<BasicDeliverEventArgs> channel;
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
            channel = provider.GetService<IChannel<BasicDeliverEventArgs>>();
            channel.BindConsumer(Executer);
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
            channel.Config(Model.Connection.Options.CunsumerMaxBatchSize, Model.Connection.Options.CunsumerMaxMillisecondsInterval);
            if (isFirst)
            {
                isFirst = false;
                Model.Model.ExchangeDeclare($"{rabbitOptions.Prefix}{Consumer.EventBus.Exchange}", "direct", true);
                Model.Model.ExchangeDeclare(Queue.Queue, "direct", true);
                Model.Model.ExchangeBind(Queue.Queue, $"{rabbitOptions.Prefix}{Consumer.EventBus.Exchange}", string.Empty);
                Model.Model.QueueDeclare(Queue.Queue, true, false, false, null);
                Model.Model.QueueBind(Queue.Queue, Queue.Queue, string.Empty);
            }
            //这里的预取数 可以做到限流的作用
            Model.Model.BasicQos(0, Model.Connection.Options.CunsumerMaxBatchSize, false);
            BasicConsumer = new EventingBasicConsumer(Model.Model);
            BasicConsumer.Received += async (ch, ea) =>
            {
                await channel.WriteAsync(ea);
            };
            BasicConsumer.ConsumerTag = Model.Model.BasicConsume(Queue.Queue, false, BasicConsumer);
            return Task.CompletedTask;
        }
        private Task Executer(List<BasicDeliverEventArgs> list)
        {
            list.ForEach(async args =>
            {
                try
                {
                    await Consumer.Notice(args.Body);
                }
                catch (Exception exception)
                {
                    Logger.LogError(exception, $"An error occurred in  consume {Queue.Queue} queue, routing path {Consumer.EventBus.Exchange}->{Queue.Queue}->{Queue.Queue}");
                    if (Consumer.Config.Reenqueue)
                    {
                        ProcessComsumerErrors(args, exception);
                        return;
                    }
                }
            });
            if (errorMessageDeliveryTags.Count == 0)
            {
                Model.Model.BasicAck(list.Max(o => o.DeliveryTag), true);
            }
            else
            {
                var waitToAck = list.Where(m => !errorMessageDeliveryTags.Contains(m.DeliveryTag)).ToList();
                waitToAck.ForEach(m =>
                {
                    Model.Model.BasicAck(m.DeliveryTag, false);
                });
            }
            return Task.CompletedTask;
        }

        private void ProcessComsumerErrors(BasicDeliverEventArgs ea, Exception exception)
        {
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
                    // 默认预取数为 300 所以每个消费者 理论上最多有 300个延时任务
                    Task.Delay((int)Math.Pow(2, retryTimes) * 1000).ContinueWith((task) =>
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
                    Model.Model.BasicAck(ea.DeliveryTag, false);
                }
            }
        }

        public void Close()
        {
            Model?.Dispose();
        }
    }
}
