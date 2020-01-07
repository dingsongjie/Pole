using Pole.ReliableMessage.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Messaging.CallBack
{
    public class MessageCallBackInfo
    {
        private Func<object, object, Task<bool>> _callBack;
        public MessageCallBackInfo(string messageTypeId, Func<object, object, Task<bool>> callBack, Type eventCallbackType, Type eventCallbackArguemntType,Type eventType)
        {
            MessageTypeId = messageTypeId;
            _callBack = callBack;
            EventCallbackType = eventCallbackType;
            EventCallbackArguemntType = eventCallbackArguemntType;
            EventType = eventType;
        }
        public string MessageTypeId { get;private set; }
        public Type EventType { get; private set; }
        public Type EventCallbackType { get; private set; }
        public Type EventCallbackArguemntType { get; private set; }

        public Task<bool> Invoke(object parameter, object reliableEvent)
        {
           return _callBack(parameter, reliableEvent);
        }
    }
}
