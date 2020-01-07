using Pole.ReliableMessage.Abstraction;
using ServiceA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Events.EventCallback
{
    public class CreateOrderReliableEventCallback : IReliableEventCallback<CreateOrderReliableEvent, int>
    {
        private readonly IOrderRepository _orderRepository;
        public CreateOrderReliableEventCallback(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public Task<bool> Callback(int callbackParemeter)
        {
            var order = _orderRepository.Get(callbackParemeter);
            //return Task.FromResult(order != null);
            return Task.FromResult(true);
        }
    }
}
