using Order.Api.Events;
using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.Masstransit;
using ServiceA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceA.Events.EventHandler
{
    [ReliableEventHandlerParemeter(PrefetchCount =16)]
    public class ToUpdateBankMoneyWhenCreateOrderReliableEventHandler : ReliableEventHandler<CreateOrderReliableEvent>
    {
        private readonly IBankRepository _bankRepository;
        public ToUpdateBankMoneyWhenCreateOrderReliableEventHandler(IServiceProvider serviceProvider, IBankRepository bankRepository) : base(serviceProvider)
        {
            _bankRepository = bankRepository;
        }

        public override async Task Handle(IReliableEventHandlerContext<CreateOrderReliableEvent> context)
        {
            await Task.CompletedTask;
            var @event = context.Event;

            //throw new Exception("test");
            _bankRepository.AddMoney(@event.OrderPrice);
        }
    }
}
