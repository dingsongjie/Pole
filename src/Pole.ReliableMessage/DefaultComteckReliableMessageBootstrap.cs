using Pole.ReliableMessage.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage
{
    class DefaultComteckReliableMessageBootstrap : IComteckReliableMessageBootstrap
    {
        private readonly IReliableEventHandlerFinder _reliableEventHandlerFinder;
        private readonly IMessageBusConfigurator _messageBusConfigurator;
        private readonly IMessageCallBackRegister _messageCallBackRegister;
        private readonly IReliableEventCallBackFinder _reliableEventCallBackFinder;
        public DefaultComteckReliableMessageBootstrap(IReliableEventHandlerFinder reliableEventHandlerFinder, IMessageBusConfigurator messageBusConfigurator, IMessageCallBackRegister messageCallBackRegister, IReliableEventCallBackFinder reliableEventCallBackFinder)
        {
            _reliableEventHandlerFinder = reliableEventHandlerFinder;
            _messageBusConfigurator = messageBusConfigurator;
            _messageCallBackRegister = messageCallBackRegister;
            _reliableEventCallBackFinder = reliableEventCallBackFinder;
        }
        public async Task Initialize(IServiceCollection services, List<Assembly> eventHandlerAssemblies, List<Assembly> eventAssemblies)
        {
            var eventHandlers = _reliableEventHandlerFinder.FindAll(eventHandlerAssemblies);
            await _messageBusConfigurator.Configure(services, eventHandlers);

            var eventCallbacks = _reliableEventCallBackFinder.FindAll(eventAssemblies);
            await _messageCallBackRegister.Register(eventCallbacks);
            RegisterEventCallbacks(services, eventCallbacks);
        }

        private void RegisterEventCallbacks(IServiceCollection services, List<Type> eventCallbacks)
        {
            eventCallbacks.ForEach(m =>
            {
                services.AddScoped(m);
            });
        }
    }
}
