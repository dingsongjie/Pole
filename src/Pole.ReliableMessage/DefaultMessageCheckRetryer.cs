using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.Storage.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage
{
    class DefaultMessageCheckRetryer : IMessageCheckRetryer
    {
        private readonly ILogger _logger;
        private readonly IRetryTimeDelayCalculator _retryTimeDelayCalculator;
        private readonly ReliableMessageOption _options;
        private readonly IMessageStorage _storage;
        private readonly IMessageChecker _messageChecker;
        private readonly IMessageBus _messageBus;
        private readonly List<Message> _changedMessage = new List<Message>();
        public DefaultMessageCheckRetryer(ILogger<DefaultMessageCheckRetryer> logger, IRetryTimeDelayCalculator retryTimeDelayCalculator, IOptions<ReliableMessageOption> options, IMessageStorage storage, IMessageChecker messageChecker, IMessageBus messageBus)
        {
            _logger = logger;
            _retryTimeDelayCalculator = retryTimeDelayCalculator;
            _options = options.Value ?? throw new Exception($"{nameof(ReliableMessageOption)} Must be injected");
            _storage = storage;
            _messageChecker = messageChecker;
            _messageBus = messageBus;
        }
        public async Task Execute(IEnumerable<Message> messages, DateTime dateTime)
        {
            try
            {
                messages.AsParallel().ForAll(async m => await Retry(m, dateTime));
                if (_changedMessage.Count != 0)
                {
                    await _storage.Save(_changedMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DefaultMessageCheckRetryer.Execute ,Execute with errors");
            }
            finally
            {
                if (_changedMessage.Count != 0)
                {
                    _changedMessage.Clear();
                }
            }
        }
        private async Task Retry(Message message, DateTime retryTime)
        {
            try
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"DefaultMessageCheckRetryer.Retry ,message:{message.Id} begin Retry");
                }
                var nextRetryDelay = _retryTimeDelayCalculator.Get(message.RetryTimes, _options.MaxPendingMessageRetryDelay);
                message.NextRetryUTCTime = retryTime.AddSeconds(nextRetryDelay);

                if (retryTime > message.AddedUTCTime.AddSeconds(_options.PendingMessageTimeOut))
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug($"DefaultMessageCheckRetryer.Retry ,message:{message.Id} would be Canced ,PendingMessageTimeOut:{_options.PendingMessageTimeOut}");
                    }

                    message.NextRetryUTCTime = DateTime.MinValue;
                    message.MessageStatus = MessageStatus.Canced;
                    _changedMessage.Add(message);
                    return;
                }
                message.RetryTimes++;

                var messageCheckerResult = await _messageChecker.GetResult(message);
                if (messageCheckerResult.IsFinished)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug($"DefaultMessageCheckRetryer.Retry ,message:{message.Id} would be Pushed");
                    }
                    message.MessageStatus = MessageStatus.Pushed;
                    await _messageBus.Publish(messageCheckerResult.Event, message.Id);
                }
                else
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug($"DefaultMessageCheckRetryer.Retry ,message:{message.Id} would be Retry next time");
                    }
                }
                _changedMessage.Add(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DefaultMessageCheckRetryer.Retry ,Message:{message.Id}  retry with errors");
            }
        }
    }
}
