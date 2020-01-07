using Pole.ReliableMessage.Abstraction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Pole.ReliableMessage.Storage.Abstraction;

namespace Pole.Pole.ReliableMessage.EventBus
{
    public class DefaultReliableBus : IReliableBus
    {
        private readonly IMessageBus _messageBus;
        private readonly IMessageStorage _messageStorage;
        private readonly IMessageIdGenerator _messageIdGenerator;
        private readonly ITimeHelper _timeHelper;
        //private readonly IMessageBuffer _messageBuffer;
        private readonly ILogger _logger;
        private readonly IJsonConverter _jsonConverter;
        private readonly IMessageCallBackInfoStore _messageCallBackInfoStore;
        private readonly IMessageTypeIdGenerator _messageTypeIdGenerator;
        public DefaultReliableBus(IMessageBus messageBus, IMessageStorage messageStorage, IMessageIdGenerator messageIdGenerator, ITimeHelper timeHelper, ILogger<DefaultReliableBus> logger, IJsonConverter jsonConverter, IMessageCallBackInfoStore messageCallBackInfoStore, IMessageTypeIdGenerator messageTypeIdGenerator)
        {
            _messageBus = messageBus;
            _messageStorage = messageStorage;
            _messageIdGenerator = messageIdGenerator;
            _timeHelper = timeHelper;
            _logger = logger;
            _jsonConverter = jsonConverter;
            _messageCallBackInfoStore = messageCallBackInfoStore;
            _messageTypeIdGenerator = messageTypeIdGenerator;
        }

        public Task<bool> Cancel<TReliableEvent>(TReliableEvent @event, string prePublishMessageId, CancellationToken cancellationToken = default)
        {
            try
            {
                return _messageStorage.UpdateStatus(m => m.Id == prePublishMessageId, MessageStatus.Canced);
            }
            catch (Exception ex)
            {
                var errorInfo = $"Cancel PrePublish errors  in defaultReliableBus;{ex.Message}";
                _logger.LogError(ex, errorInfo);
                throw new Exception(errorInfo, ex);
            }
        }

        public async Task<string> PrePublish<TReliableEvent>(TReliableEvent @event, object callbackParemeter, CancellationToken cancellationToken = default)
        {

            var messageTypeId = _messageTypeIdGenerator.Generate(typeof(TReliableEvent));

            var currentMessageCallbackInfo = _messageCallBackInfoStore.Get(messageTypeId);
            if (currentMessageCallbackInfo == null)
            {
                throw new Exception($"Current message type Callback not registered ,messageTypeId:{messageTypeId}");
            }
            try
            {
                var messageId = _messageIdGenerator.Generate();

                _logger.LogDebug($"PrePublish message begin ,messageId:{messageId}");

                var now = _timeHelper.GetUTCNow();
                var content = _jsonConverter.Serialize(@event);
                var callBackParem = _jsonConverter.Serialize(callbackParemeter);
                Message newMessage = new Message()
                {
                    AddedUTCTime = now,
                    Content = content,
                    Id = messageId,
                    MessageStatusId = MessageStatus.Pending.Id,
                    MessageTypeId = messageTypeId,
                    RePushCallBackParameterValue = callBackParem,
                    NextRetryUTCTime = DateTime.MinValue
                };
                await _messageStorage.Add(newMessage);

                _logger.LogDebug($"PrePublish message successful ,messageId:{messageId}");

                return messageId;
            }
            catch (Exception ex)
            {
                var errorInfo = $"PrePublish errors in DefaultReliableBus;{ex.Message}";
                _logger.LogError(ex, errorInfo);
                throw new Exception(errorInfo, ex);
            }
        }

        public async Task<bool> Publish<TReliableEvent>(TReliableEvent @event, string prePublishMessageId, CancellationToken cancellationToken = default)
        {
            try
            {
                await _messageBus.Publish(@event, prePublishMessageId, cancellationToken);

                var messageBufferResult = await _messageStorage.UpdateStatus(m => m.Id == prePublishMessageId && m.MessageStatusId == MessageStatus.Pending.Id, MessageStatus.Pushed);
                return messageBufferResult;
            }
            catch (Exception ex)
            {
                var errorInfo = $"Publish errors in DefaultReliableBus;{ex.Message}";
                _logger.LogError(ex, errorInfo);
                throw new Exception(errorInfo, ex);
            }
        }
    }
}
