using Pole.Application.EventBus;
using Pole.ReliableMessage.Abstraction;
using Product.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.Application.IntegrationEvent.Handler
{
    public class JustTestWhenProductAddedIntegrationEventHandler : IntegrationEventHandler<ProductAddedIntegrationEvent>
    {
        public JustTestWhenProductAddedIntegrationEventHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override Task Handle(IReliableEventHandlerContext<ProductAddedIntegrationEvent> context)
        {
            try
            {

            }
            catch(Exception ex)
            {

            }
            return Task.FromResult(1);
        }
    }
}
