using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Pole.ReliableMessage.Storage.Abstraction;

namespace Pole.ReliableMessage.Processor
{
    class PendingMessageCheckProcessor : ProcessorBase
    {
        private readonly IMessageStorage _storage;
        private readonly ReliableMessageOption _options;
        private readonly IMessageBuffer _messageBuffer;
        private readonly ITimeHelper _timeHelper;
        private readonly IMessageChecker _messageChecker;
        private readonly IRetryTimeDelayCalculator _retryTimeDelayCalculator;
        private readonly IMemberShipTable _memberShipTable;
        private readonly ILogger<PendingMessageCheckProcessor> _logger;
        private readonly IServiceIPv4AddressProvider _serviceIPv4AddressProvider;
        private readonly IMessageBus _messageBus;
        public PendingMessageCheckProcessor(IMessageStorage storage, IOptions<ReliableMessageOption> options, IMessageBuffer messageBuffer, ITimeHelper timeHelper, IMessageChecker messageChecker, IRetryTimeDelayCalculator retryTimeDelayCalculator, IMemberShipTable memberShipTable, ILogger<PendingMessageCheckProcessor> logger, IServiceIPv4AddressProvider serviceIPv4AddressProvider, IMessageBus messageBus)
        {
            _storage = storage;
            _options = options.Value ?? throw new Exception($"{nameof(ReliableMessageOption)} Must be injected");
            _messageBuffer = messageBuffer;
            _timeHelper = timeHelper;
            _messageChecker = messageChecker;
            _retryTimeDelayCalculator = retryTimeDelayCalculator;
            _memberShipTable = memberShipTable;
            _logger = logger;
            _serviceIPv4AddressProvider = serviceIPv4AddressProvider;
            _messageBus = messageBus;
        }
        public override string Name => nameof(PendingMessageCheckProcessor);


        public override async Task Process(ProcessingContext context)
        {
            try
            {
                var iPStr = _serviceIPv4AddressProvider.Get();

                var isPendingChecker = await _memberShipTable.IsPendingMessageCheckerServiceInstance(iPStr);// 这里可以把时间加上去 
                if (!isPendingChecker)
                {
                    _logger.LogDebug("I an not the PendingChecker ,Ignore check");
                    return;
                }

                var now = _timeHelper.GetUTCNow();
                var pendingMessages = await _storage.GetMany(m => m.MessageStatusId == MessageStatus.Pending.Id &&m.NextRetryUTCTime <= now && m.AddedUTCTime <= now.AddSeconds(-1 * _options.PendingMessageFirstProcessingWaitTime)&&m.AddedUTCTime>= now.AddSeconds(-1 * _options.PendingMessageCheckingTimeOutSeconds),_options.PendingMessageCheckBatchCount);

                var cachedPushedMessage = await _messageBuffer.GetAll(m => m.MessageStatus == MessageStatus.Pushed);

                var finalToCheckedMessages = pendingMessages.Except(cachedPushedMessage, MessageIEqualityComparer.Default).ToList();
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"PendingMessageCheckProcessor  pendingMessages count:{pendingMessages.Count}");
                    _logger.LogDebug($"PendingMessageCheckProcessor  cachedPushedMessage count:{cachedPushedMessage.Count}");
                    _logger.LogDebug($"PendingMessageCheckProcessor  finalToCheckedMessages count:{finalToCheckedMessages.Count}");
                }

                finalToCheckedMessages.AsParallel().ForAll(async m => await Retry(m, now));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PendingMessageCheckProcessor Process Error");
            }
            finally
            {
                await Task.Delay(_options.PendingMessageRetryInterval * 1000);
            }
        }

        private async Task Retry(Message message, DateTime retryTime)
        {
            try
            {
                await Task.CompletedTask;

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"PendingMessageCheckProcessor.Retry ,message:{message.Id} begin Retry");
                }
                var nextRetryDelay = _retryTimeDelayCalculator.Get(message.RetryTimes, _options.MaxPendingMessageRetryDelay);
                message.NextRetryUTCTime = retryTime.AddSeconds(nextRetryDelay);

                if (retryTime > message.AddedUTCTime.AddSeconds(_options.PendingMessageTimeOut))
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug($"PendingMessageCheckProcessor.Retry ,message:{message.Id} would be Canced ,PendingMessageTimeOut:{_options.PendingMessageTimeOut}");
                    }

                    message.NextRetryUTCTime = DateTime.MinValue;
                    message.MessageStatus = MessageStatus.Canced;
                    await _messageBuffer.Add(message);
                    return;
                }
                message.RetryTimes++;

                var messageCheckerResult = await _messageChecker.GetResult(message);
                if (messageCheckerResult.IsFinished)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug($"PendingMessageCheckProcessor.Retry ,message:{message.Id} would be Pushed");
                    }
                    message.MessageStatus = MessageStatus.Pushed;
                    await _messageBus.Publish(messageCheckerResult.Event, message.Id);
                }
                else
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        _logger.LogDebug($"PendingMessageCheckProcessor.Retry ,message:{message.Id} would be Retry next time");
                    }
                }
                await _messageBuffer.Add(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PendingMessageCheckProcessor Retry ,Message:{message.Id}  retry with errors");
            }
        }
    }
}
