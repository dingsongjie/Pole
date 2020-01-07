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
        //private readonly IMessageBuffer _messageBuffer;
        private readonly ITimeHelper _timeHelper;
        private readonly IMemberShipTable _memberShipTable;
        private readonly ILogger<PendingMessageCheckProcessor> _logger;
        private readonly IServiceIPv4AddressProvider _serviceIPv4AddressProvider;
        private readonly IMessageCheckRetryer _messageCheckRetryer;
        public PendingMessageCheckProcessor(IMessageStorage storage, IOptions<ReliableMessageOption> options, ITimeHelper timeHelper, IMemberShipTable memberShipTable, ILogger<PendingMessageCheckProcessor> logger, IServiceIPv4AddressProvider serviceIPv4AddressProvider, IMessageCheckRetryer messageCheckRetryer)
        {
            _storage = storage;
            _options = options.Value ?? throw new Exception($"{nameof(ReliableMessageOption)} Must be injected");
            //_messageBuffer = messageBuffer;
            _timeHelper = timeHelper;
            _memberShipTable = memberShipTable;
            _logger = logger;
            _serviceIPv4AddressProvider = serviceIPv4AddressProvider;
            _messageCheckRetryer = messageCheckRetryer;
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

                await ProcessInternal();
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
        public async Task ProcessInternal()
        {
            var now = _timeHelper.GetUTCNow();
            var pendingMessages = await _storage.GetMany(m => m.MessageStatusId == MessageStatus.Pending.Id && m.NextRetryUTCTime <= now && m.AddedUTCTime <= now.AddSeconds(-1 * _options.PendingMessageFirstProcessingWaitTime) && m.AddedUTCTime >= now.AddSeconds(-1 * _options.PendingMessageCheckingTimeOutSeconds), _options.PendingMessageCheckBatchCount);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"PendingMessageCheckProcessor  pendingMessages count:{pendingMessages.Count}");
            }

            await _messageCheckRetryer.Execute(pendingMessages, now);
            // 说明此时 消息数量超过 批量获取数 
            if (pendingMessages.Count == _options.PendingMessageCheckBatchCount)
            {
                await ProcessInternal();
            }
        }
    }
}
