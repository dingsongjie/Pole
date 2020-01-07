using Pole.Pole.ReliableMessage.EventBus;
using Pole.ReliableMessage;
using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.EventBus;
using Pole.ReliableMessage.Messaging;
using Pole.ReliableMessage.Messaging.CallBack;
using Pole.ReliableMessage.Processor;
using Pole.ReliableMessage.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ComteckReliableMessageServiceCollectionExtensions
    {
        public static IServiceCollection AddComteckReliableMessage(this IServiceCollection  services, Action<ReliableMessageOption> optionConfig)
        {
            ReliableMessageOption reliableMessageOption = new ReliableMessageOption();
            optionConfig(reliableMessageOption);

            foreach(var extension in reliableMessageOption.ReliableMessageOptionExtensions)
            {
                extension.AddServices(services);
            }
            services.Configure(optionConfig);

            services.AddSingleton<IJsonConverter, DefaultJsonConverter>();
            services.AddSingleton<IRetryTimeDelayCalculator, DefaultRetryTimeDelayCalculator>();
            services.AddSingleton<ITimeHelper, DefaulTimeHelper>();
            services.AddSingleton<IApplicationBuilderConfigurator, DefaultApplicationBuilderConfigurator>();
            services.AddSingleton<IComteckReliableMessageBootstrap, DefaultComteckReliableMessageBootstrap>();
            services.AddSingleton<IMessageCallBackInfoGenerator, DefaultMessageCallBackInfoGenerator>();
            services.AddSingleton<IMessageCallBackInfoStore, MessageCallBackInfoInMemoryStore>();
            services.AddSingleton<IMessageCallBackRegister, DefaultMessageCallBackRegister>();
            services.AddSingleton<IMessageChecker, DefaultMessageChecker>();
            services.AddSingleton<IMessageCallBackInfoGenerator, DefaultMessageCallBackInfoGenerator>();
            services.AddSingleton<IReliableBus, DefaultReliableBus>();
            services.AddSingleton<IReliableEventCallBackFinder, DefaultReliableEventCallBackFinder>();
            services.AddSingleton<IReliableEventHandlerFinder, DefaultReliableEventHandlerFinder>();
            services.AddSingleton<IMessageTypeIdGenerator, DefaultMessageTypeIdGenerator>();
            services.AddSingleton<IServiceIPv4AddressProvider, DefaultServiceIPv4AddressProvider>();

            services.AddHostedService<BackgroundServiceBasedProcessorServer>();

            services.AddHttpClient();




            services.AddSingleton<IProcessor, MessageCleanProcessor>();
            services.AddSingleton<IProcessor, PendingMessageCheckProcessor>();
            services.AddSingleton<IProcessor, PendingMessageServiceInstanceCheckProcessor>();

            var provider = services.BuildServiceProvider();

            IComteckReliableMessageBootstrap applicationBuilderConfigurator = provider.GetService(typeof(IComteckReliableMessageBootstrap)) as IComteckReliableMessageBootstrap;

            applicationBuilderConfigurator.Initialize(services, reliableMessageOption.EventHandlerAssemblies, reliableMessageOption.EventCallbackAssemblies);
            return services; 
        }     
    }
}
