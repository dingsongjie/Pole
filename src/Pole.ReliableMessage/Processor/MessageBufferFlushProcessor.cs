using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Processor
{
    public class MessageBufferFlushProcessor : ProcessorBase
    {
        private readonly IMessageBuffer _messageBuffer;
        private readonly ReliableMessageOption _options;
        private readonly ILogger<MessageBufferFlushProcessor> _logger;
        public MessageBufferFlushProcessor(IMessageBuffer messageBuffer, IOptions<ReliableMessageOption> options, ILogger<MessageBufferFlushProcessor> logger)
        {
            _messageBuffer = messageBuffer;
            _options = options.Value ?? throw new Exception($"{nameof(ReliableMessageOption)} Must be injected");
            _logger = logger;
        }
        public override string Name => nameof(PendingMessageCheckProcessor);

        public override async Task Process(ProcessingContext context)
        {
            try
            {
                await _messageBuffer.Flush();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"MessageBufferFlushProcessor Process Error");
            }
            finally
            {
                await Task.Delay(_options.PushedMessageFlushInterval * 1000);
            }
        }
    }
}
