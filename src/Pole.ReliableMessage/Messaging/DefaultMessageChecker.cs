using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.Messaging;
using Pole.ReliableMessage.Messaging.CallBack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Pole.ReliableMessage.Storage.Abstraction;

namespace Pole.ReliableMessage.Messaging
{
    class DefaultMessageChecker : IMessageChecker
    {
        private readonly IMessageCallBackInfoStore _messageCallBackInfoStore;
        private readonly ILogger<DefaultMessageChecker> _logger;
        private readonly IJsonConverter _jsonConverter;
        private readonly IServiceProvider _serviceProvider;
        public DefaultMessageChecker(IMessageCallBackInfoStore messageCallBackInfoStore, ILogger<DefaultMessageChecker> logger, IJsonConverter jsonConverter, IServiceProvider serviceProvider)
        {
            _messageCallBackInfoStore = messageCallBackInfoStore;
            _logger = logger;
            _jsonConverter = jsonConverter;
            _serviceProvider = serviceProvider;
        }
        public async Task<MessageCheckerResult> GetResult(Message message)
        {
            try
            {
                var callBackInfo = await _messageCallBackInfoStore.Get(message.MessageTypeId);
                if (callBackInfo == null)
                {
                    _logger.LogError($"Can't find the callBackInfo, MessageTypeId:{message.MessageTypeId}");
                    return  MessageCheckerResult.NotFinished;
                }
                using (var scope = _serviceProvider.CreateScope())
                {
                    var callback = scope.ServiceProvider.GetRequiredService(callBackInfo.EventCallbackType);
                    var argument = _jsonConverter.Deserialize(message.RePushCallBackParameterValue, callBackInfo.EventCallbackArguemntType);
                    var result = await callBackInfo.Invoke(argument, callback);
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        var messageInfoDetail = _jsonConverter.Serialize(message);
                        _logger.LogDebug($"DefaultMessageChecker IsFinished Result:{result.ToString()},MessageTypeId:{message.MessageTypeId},MessageDetail:{messageInfoDetail}");
                    }
                    if (result)
                    {
                        var @event = _jsonConverter.Deserialize(message.Content, callBackInfo.EventType);
                        return new MessageCheckerResult(true, @event);
                    }
                    return MessageCheckerResult.NotFinished;
                }
            }
            catch (Exception ex)
            {
                var messageInfoDetail = _jsonConverter.Serialize(message);
                _logger.LogError(ex, $"DefaultMessageChecker.IsFinished  Error, MessageTypeId:{message.MessageTypeId},MessageDetail:{messageInfoDetail}");
                return MessageCheckerResult.NotFinished;
            }
        }
    }
}
