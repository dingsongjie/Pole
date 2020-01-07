using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.Masstransit.Pipe;
using Pole.ReliableMessage.Messaging;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Pole.ReliableMessage.Storage.Abstraction;

namespace Pole.ReliableMessage.Masstransit
{

    public abstract class ReliableEventHandler<TEvent> : IReliableEventHandler<TEvent>, IConsumer<TEvent>
       where TEvent : class
    {
        private readonly IMessageStorage _messageStorage;
        private readonly ILogger<ReliableEventHandler<TEvent>> _logger;
        private readonly IServiceProvider _serviceProvider;
        public ReliableEventHandler(IServiceProvider serviceProvider)
        {
            _messageStorage = serviceProvider.GetRequiredService<IMessageStorage>();
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<ReliableEventHandler<TEvent>>();
            _serviceProvider = serviceProvider;
        }

        public abstract Task Handle(IReliableEventHandlerContext<TEvent> context);
        public async Task Consume(ConsumeContext<TEvent> context)
        {
            var messageId = GetReliableMessageId(context);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var jsonConveter = _serviceProvider.GetRequiredService(typeof(IJsonConverter)) as IJsonConverter;
                var json = jsonConveter.Serialize(context.Message);
                _logger.LogDebug($"Message Begin Handle,messageId:{messageId}, message content :{json}");
            }

            var retryAttempt = context.GetRetryAttempt();
            if (retryAttempt == 0)
            {
                if (string.IsNullOrEmpty(messageId))
                {
                    _logger.LogWarning($"Message has no ReliableMessageId, ignore");
                    return;
                }
                var isHandled = !await _messageStorage.CheckAndUpdateStatus(m => m.Id == messageId, MessageStatus.Handed);
                if (isHandled)
                {
                    _logger.LogTrace($"This message has handled begore ReliableMessageId:{messageId}, ignore");
                    return;
                }
            }
            await Handle(new DefaultReliableEventHandlerContext<TEvent>(context));

            _logger.LogDebug($"Message handled successfully ,messageId:{messageId}");
        }

        private string GetReliableMessageId(ConsumeContext<TEvent> context)
        {
            return context.Headers.Get(AddReliableMessageIdPipe.RELIABLE_MESSAGE_ID, string.Empty);
        }
    }
}
