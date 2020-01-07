using Pole.ReliableMessage.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pole.ReliableMessage.Storage.Abstraction;

namespace Pole.ReliableMessage.Processor
{
    class PendingMessageServiceInstanceCheckProcessor : ProcessorBase
    {
        private readonly ReliableMessageOption _options;
        private readonly ITimeHelper _timeHelper;
        private readonly IMemberShipTableManager _memberShipTable;
        private readonly ILogger<PendingMessageServiceInstanceCheckProcessor> _logger;
        private readonly IServiceIPv4AddressProvider _serviceIPv4AddressProvider;
        public PendingMessageServiceInstanceCheckProcessor(IOptions<ReliableMessageOption> options, ITimeHelper timeHelper, IMemberShipTableManager memberShipTable, ILogger<PendingMessageServiceInstanceCheckProcessor> logger, IServiceIPv4AddressProvider serviceIPv4AddressProvider)
        {
            _options = options.Value ?? throw new Exception($"{nameof(ReliableMessageOption)} Must be injected");
            _timeHelper = timeHelper;
            _memberShipTable = memberShipTable;
            _logger = logger;
            _serviceIPv4AddressProvider = serviceIPv4AddressProvider;
        }
        public override string Name => nameof(PendingMessageCheckProcessor);


        public override async Task Process(ProcessingContext context)
        {
            try
            {
                var now = _timeHelper.GetUTCNow();
                var iPStr = _serviceIPv4AddressProvider.Get();
                _logger.LogDebug($"Current instance ip is {iPStr}");

                var currentCheckIp = await _memberShipTable.GetPendingMessageCheckerServiceInstanceIp(now.AddSeconds(-1 * _options.PendingMessageCheckerInstanceIAnAliveTimeout));
                if (string.IsNullOrEmpty(currentCheckIp))
                {
                    var addInstanceResult = await _memberShipTable.AddCheckerServiceInstanceAndDeleteOthers(iPStr, now);
                    if (addInstanceResult)
                    {
                        _logger.LogInformation($"I am the PendingMessageCheck now, ip:{iPStr}");
                        return;
                    }
                    _logger.LogError($"There is no PendingMessageChecker ,I want to be the PendingMessageCheck ,but faild ,memberShipTable.AddCheckerServiceInstance faild  , ip:{iPStr}");
                    return;
                }
                if (currentCheckIp == iPStr)
                {
                    _logger.LogDebug($"Begin update pendingMessageChecker iAmAliveUTCTime");
                    await _memberShipTable.UpdateIAmAlive(currentCheckIp, now);
                    _logger.LogDebug($"Update pendingMessageChecker iAmAliveUTCTime successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"PendingMessageServiceInstanceCheckProcessor Process Error");
            }
            finally
            {
                await Task.Delay(_options.PendingMessageCheckerInstanceIAnAliveTimeUpdateDelay * 1000);
            }
        }
    }
}
