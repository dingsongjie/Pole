using Backet.Api.Domain.AggregatesModel.BacketAggregate;
using Pole.Application.EventBus;
using Pole.Domain.UnitOfWork;
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
        private readonly IBacketRepository _backetRepository;
        private readonly IUnitOfWork _unitOfWork;
        public JustTestWhenProductAddedIntegrationEventHandler(IServiceProvider serviceProvider, IBacketRepository backetRepository, IUnitOfWork unitOfWork) : base(serviceProvider)
        {
            _backetRepository = backetRepository;
            _unitOfWork = unitOfWork;
        }

        public override async Task Handle(IReliableEventHandlerContext<ProductAddedIntegrationEvent> context)
        {

            var @event = context.Event;
            Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet backet = new Domain.AggregatesModel.BacketAggregate.Backet(@event.BacketId, "1");
            backet.AddBacketItem(@event.ProductId, @event.ProductName, @event.Price);
            _backetRepository.Add(backet);
            await _backetRepository.SaveEntitiesAsync();

            await _unitOfWork.CompeleteAsync();
        }
    }
}
