using Pole.ReliableMessage.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Messaging.CallBack
{
    class DefaultMessageCallBackRegister : IMessageCallBackRegister
    {
        private readonly IMessageCallBackInfoGenerator _messageCallBackInfoGenerator;
        private readonly IMessageCallBackInfoStore _messageCallBackInfoStore;
        public DefaultMessageCallBackRegister(IMessageCallBackInfoGenerator messageCallBackInfoGenerator, IMessageCallBackInfoStore messageCallBackInfoStore)
        {
            _messageCallBackInfoGenerator = messageCallBackInfoGenerator;
            _messageCallBackInfoStore = messageCallBackInfoStore;
        }
        public async Task Register(IEnumerable<Type> eventCallbackTypes)
        {
            foreach(var eventCallbackType in eventCallbackTypes)
            {
                var messageCallBackInfo = _messageCallBackInfoGenerator.Generate(eventCallbackType);
                await _messageCallBackInfoStore.Add(messageCallBackInfo);
            }
        }
    }
}
