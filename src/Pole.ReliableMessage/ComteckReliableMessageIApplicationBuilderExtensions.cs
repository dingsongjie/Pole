using Pole.ReliableMessage;
using Pole.ReliableMessage.Abstraction;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.AspNetCore.Builder
{
    public static class ComteckReliableMessageIApplicationBuilderExtensions
    {
        public static IApplicationBuilder UsePoleReliableMessage(this IApplicationBuilder  applicationBuilder)
        {
            var option = applicationBuilder.ApplicationServices.GetRequiredService(typeof(IOptions<ReliableMessageOption>)) as IOptions<ReliableMessageOption>;
            var messageCallBackRegister = applicationBuilder.ApplicationServices.GetRequiredService(typeof(IMessageCallBackRegister)) as IMessageCallBackRegister;
            var reliableEventCallBackFinder = applicationBuilder.ApplicationServices.GetRequiredService(typeof(IReliableEventCallBackFinder)) as IReliableEventCallBackFinder;

            var eventCallbacks = reliableEventCallBackFinder.FindAll(option.Value.EventCallbackAssemblies);
            messageCallBackRegister.Register(eventCallbacks).GetAwaiter().GetResult();

            return applicationBuilder;
        }
    }
}
