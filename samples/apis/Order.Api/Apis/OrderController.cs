using Microsoft.AspNetCore.Mvc;
using Order.Api.Events;
using Pole.ReliableMessage.Abstraction;
using ServiceA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IReliableBus _reliableBus;
        private readonly IOrderRepository _orderRepository;
        public OrderController(IReliableBus reliableBus, IOrderRepository orderRepository)
        {
            _reliableBus = reliableBus;
            _orderRepository = orderRepository;
        }
        [HttpGet]
        public async Task<string> Index()
        {
            var order = new Order.Api.Entity.Order
            {
                Id = 1,
                Price = 10
            };
            CreateOrderReliableEvent createOrderReliableEvent = new CreateOrderReliableEvent
            {
                OrderPrice = order.Price
            };
            var prePublishId = await _reliableBus.PrePublish(createOrderReliableEvent, order.Id);
            _orderRepository.Add(order);

            await _reliableBus.Publish(createOrderReliableEvent, prePublishId);
            return "ok";
        }
    }
}