using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.EventBus;
using Pole.ReliableMessage.Masstransit.Abstraction;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Masstransit
{
    class MasstransitMessageBusConfigurator : IMessageBusConfigurator
    {
        private readonly IReliableEventHandlerRegistrarFactory _reliableEventHandlerRegistrarFactory;
        private readonly MasstransitRabbitmqOption _options;
        public MasstransitMessageBusConfigurator(IReliableEventHandlerRegistrarFactory reliableEventHandlerRegistrarFactory, IOptions<MasstransitRabbitmqOption> options)
        {
            _reliableEventHandlerRegistrarFactory = reliableEventHandlerRegistrarFactory;
            _options = options.Value;
        }
        public async Task Configure(IServiceCollection services,IEnumerable<Type> eventHandlerTypes)
        {
            await Task.CompletedTask;
            var eventHandlerRegistrars = GetEventHandlerRegistrars(eventHandlerTypes).ToList();
            services.AddMassTransit(x =>
            {
                foreach (var eventHandlerRegistrar in eventHandlerRegistrars)
                {
                    eventHandlerRegistrar.RegisterEventHandler(x, services);
                }
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri(_options.RabbitMqHostAddress), h =>
                    {
                        h.Username(_options.RabbitMqHostUserName);
                        h.Password(_options.RabbitMqHostPassword);
                        
                    });
                    foreach (var eventHandlerRegistrar in eventHandlerRegistrars)
                    {
                        eventHandlerRegistrar.RegisterQueue(x, cfg, host, provider);
                    }
                }));
            });
        }
        private IEnumerable<MasstransitEventHandlerRegistrar> GetEventHandlerRegistrars(IEnumerable<Type> eventHandlerTypes)
        {
            foreach (var eventHandler in eventHandlerTypes)
            {
                var model = _reliableEventHandlerRegistrarFactory.Create(eventHandler);
                yield return model;
            }
        }
    }
}
