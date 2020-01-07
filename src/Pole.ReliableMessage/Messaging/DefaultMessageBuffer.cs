using Pole.ReliableMessage.Abstraction;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Messaging
{
    class DefaultMessageBuffer : IMessageBuffer
    {
        private readonly IMessageStorage _storage;
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string,Message> Messages = new System.Collections.Concurrent.ConcurrentDictionary<string, Message>();
        private readonly ILogger<DefaultMessageBuffer> _logger;
        public DefaultMessageBuffer(IMessageStorage storage, ILogger<DefaultMessageBuffer> logger)
        {
            _storage = storage;
            _logger = logger;
        }
        public async Task Flush()
        {
            /// 通过 MessageTypeId 是否为空 判断 消息是否为 DefaultReliableBus Publish 完成后的消息状态修改缓冲, 
            var toUpdateStatusMessageKeyValuePairs = Messages.Where(m => string.IsNullOrEmpty(m.Value.MessageTypeId));
            var toUpdateStatusMessages= toUpdateStatusMessageKeyValuePairs.Select(m=>m.Value).ToArray();
            var toUpdateStatusMessageIds = toUpdateStatusMessageKeyValuePairs.Select(m => m.Key).ToList();
            await _storage.UpdateStatus(toUpdateStatusMessages);

            _logger.LogDebug($"DefaultMessageBuffer.Flush update successfully, Message count{toUpdateStatusMessages.Count()}");
            toUpdateStatusMessageIds.ForEach(m => {
                Messages.TryRemove(m,out Message message);
            });

            var toSavedMessageKeyValuePairs = Messages.Where(m => !string.IsNullOrEmpty(m.Value.MessageTypeId));
            var toSavedMessages = toSavedMessageKeyValuePairs.Select(m => m.Value).ToArray();
            var toSavedMessagesIds = toSavedMessageKeyValuePairs.Select(m => m.Key).ToList();
            await _storage.Save(toSavedMessages);

            _logger.LogDebug($"DefaultMessageBuffer.Flush save successfully, Message count{toSavedMessages.Count()}");
            toSavedMessagesIds.ForEach(m => {
                Messages.TryRemove(m, out Message message);
            });
        }

        public Task<bool> Add(Message message)
        {
            Messages.TryAdd(message.Id, message);
            return Task.FromResult(true);
        }

        public async Task<List<Message>> GetAll(Func<Message, bool> filter)
        {
            await Task.CompletedTask;
            return Messages.Values.Where(filter).ToList();
        }
    }
}
