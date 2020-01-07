using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.Messaging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Pole.ReliableMessage.Storage.Abstraction;

namespace Pole.ReliableMessage.Processor
{
    public class MessageCleanProcessor : ProcessorBase
    {
        private readonly ReliableMessageOption _options;
        private readonly ILogger<MessageCleanProcessor> _logger;
        private readonly IMessageStorage _messageStorage;
        private readonly IMemberShipTable _memberShipTable;
        private readonly IServiceIPv4AddressProvider _serviceIPv4AddressProvider;
        public MessageCleanProcessor(IOptions<ReliableMessageOption> options, ILogger<MessageCleanProcessor> logger, IMessageStorage messageStorage, IMemberShipTable memberShipTable, IServiceIPv4AddressProvider serviceIPv4AddressProvider)
        {
            _options = options.Value ?? throw new Exception($"{nameof(ReliableMessageOption)} Must be injected");
            _logger = logger;
            _messageStorage = messageStorage;
            _memberShipTable = memberShipTable;
            _serviceIPv4AddressProvider = serviceIPv4AddressProvider;
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
                    _logger.LogDebug("I an not the pendingChecker ,Ignore clean up messages");
                    return;
                }
                _logger.LogInformation($"Begin clean message");

                var deletedCount = await _messageStorage.Delete(m => m.MessageStatusId == MessageStatus.Canced.Id || m.MessageStatusId == MessageStatus.Handed.Id);

                _logger.LogInformation($"End clean message ,delete message count : {deletedCount} , successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Clean message error");
            }
            finally
            {
                await Task.Delay(_options.MessageCleanInterval * 1000);
            }
        }
    }
}
