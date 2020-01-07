using Pole.ReliableMessage.Abstraction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Messaging.CallBack
{
    public class MessageCallBackInfoInMemoryStore : Dictionary<string, MessageCallBackInfo>, IMessageCallBackInfoStore
    {
        private readonly ILogger<MessageCallBackInfoInMemoryStore> _logger;
        public MessageCallBackInfoInMemoryStore(ILogger<MessageCallBackInfoInMemoryStore> logger)
        {
            _logger = logger;
        }
        public async Task Add(MessageCallBackInfo messageCallBackInfo)
        {
            await Task.CompletedTask;
            if (TryGetValue(messageCallBackInfo.MessageTypeId, out MessageCallBackInfo info))
            {
                throw new Exception($"Add MessageCallBackInfo Faild , MessageCallBackInfo Already Added ,MessageTypeId:{messageCallBackInfo.MessageTypeId}");
            }
            Add(messageCallBackInfo.MessageTypeId, messageCallBackInfo);
            _logger.LogDebug($"Add MessageCallBackInfo Success ,MessageTypeId:{messageCallBackInfo.MessageTypeId}");
        }

        public async Task<MessageCallBackInfo> Get(string messageTypeId)
        {
            await Task.CompletedTask;
            if (TryGetValue(messageTypeId, out MessageCallBackInfo info))
            {
                return info;
            }
            return null;
        }
    }
}
