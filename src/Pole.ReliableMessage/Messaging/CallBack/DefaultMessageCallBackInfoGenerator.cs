using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Messaging.CallBack
{
    class DefaultMessageCallBackInfoGenerator : IMessageCallBackInfoGenerator
    {
        private readonly IMessageTypeIdGenerator _messageTypeIdGenerator;
        public DefaultMessageCallBackInfoGenerator(IMessageTypeIdGenerator messageTypeIdGenerator)
        {
            _messageTypeIdGenerator = messageTypeIdGenerator;
        }
        public MessageCallBackInfo Generate(Type eventCallbackType)
        {
            var @interface = eventCallbackType.GetInterfaces().FirstOrDefault();
            Func<object, object, Task<bool>> deleg = MakeCallBackFunc(eventCallbackType, @interface);

            var eventType = @interface.GetGenericArguments()[0];
            var eventCallbackArguemntType = @interface.GetGenericArguments()[1];
            var enentName = _messageTypeIdGenerator.Generate(eventType);

            MessageCallBackInfo messageCallBackInfo = new MessageCallBackInfo(enentName, deleg, eventCallbackType, eventCallbackArguemntType, eventType);
            return messageCallBackInfo;
        }

        private static Func<object, object, Task<bool>> MakeCallBackFunc(Type eventType, Type @interface)
        {
            var callbackParemeterType = @interface.GetGenericArguments()[1];
            var argument = Expression.Parameter(typeof(object));
            var paremeter = Expression.Parameter(typeof(object));
           // var typedParemeter = Expression.Parameter(eventType);
            var typedcallbackParemeter = Expression.Convert(argument, callbackParemeterType);

            var typedParemeter = Expression.Convert(paremeter, eventType);

            var callBackMethod = eventType.GetMethod("Callback");
            var call = Expression.Call(typedParemeter, callBackMethod, typedcallbackParemeter);

            //var innerParemeter = eventType.GetInterfaces().FirstOrDefault();
            var lambda = Expression.Lambda<Func<object, object, Task<bool>>>(call, true, argument, paremeter);
            var deleg = lambda.Compile();
            return deleg;
        }
    }
}
