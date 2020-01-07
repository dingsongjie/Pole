using Pole.ReliableMessage;
using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.Masstransit;
using Pole.ReliableMessage.Masstransit.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Pole.ReliableMessage.Masstransit.Messaging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ReliableMessageOptionExtension
    {
        public static ReliableMessageOption AddMasstransitRabbitmq(this ReliableMessageOption option, Action<MasstransitRabbitmqOption> optionConfig)
        {
            option.ReliableMessageOptionExtensions.Add(new MasstransitRabbitmqExtension(optionConfig));
            return option;
        }
    }
    public class MasstransitRabbitmqExtension : IReliableMessageOptionExtension
    {
        private readonly Action<MasstransitRabbitmqOption> _masstransitRabbitmqOption;
        public MasstransitRabbitmqExtension(Action<MasstransitRabbitmqOption> masstransitRabbitmqOption)
        {
            _masstransitRabbitmqOption = masstransitRabbitmqOption;
        }
        public void AddServices(IServiceCollection services)
        {
            services.Configure(_masstransitRabbitmqOption);
            services.AddSingleton<IMessageBus, MasstransitBasedMessageBus>();
            services.AddSingleton<IMessageBusConfigurator, MasstransitMessageBusConfigurator>();
            services.AddSingleton<IReliableEventHandlerRegistrarFactory, DefaultReliableEventHandlerRegistrarFactory>();
            services.AddSingleton<IMessageIdGenerator, DefaultMessageIdGenerator>();
            services.AddHostedService<MassTransitHostedService>();
        }
    }
}
