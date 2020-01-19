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
        private readonly ILogger<ReliableEventHandler<TEvent>> _logger;
        public ReliableEventHandler(IServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            _logger = loggerFactory.CreateLogger<ReliableEventHandler<TEvent>>();
        }

        public abstract Task Handle(IReliableEventHandlerContext<TEvent> context);
        public async Task Consume(ConsumeContext<TEvent> context)
        {
            var messageId = GetReliableMessageId(context);

            _logger.LogDebug($"Message Begin Handle,messageId:{messageId}");

            await Handle(new DefaultReliableEventHandlerContext<TEvent>(context));

            _logger.LogDebug($"Message handled successfully ,messageId:{messageId}");
        }

        private string GetReliableMessageId(ConsumeContext<TEvent> context)
        {
            return context.Headers.Get(AddReliableMessageIdPipe.RELIABLE_MESSAGE_ID, string.Empty);
        }
    }
}
