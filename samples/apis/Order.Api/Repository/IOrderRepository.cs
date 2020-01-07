using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceA.Repository
{
    public interface IOrderRepository
    {
        Order.Api.Entity.Order Get(int id);
        bool Add(Order.Api.Entity.Order order);
        bool Save(Order.Api.Entity.Order order);
    }
    class OrderRepository : IOrderRepository
    {
        private static ConcurrentBag<Order.Api.Entity.Order> Orders = new ConcurrentBag<Order.Api.Entity.Order>();
        public OrderRepository()
        {
            Orders.Add(new Order.Api.Entity.Order { Id = 1, Price = 12 });
        }
        public Order.Api.Entity.Order Get(int id)
        {
            return Orders.FirstOrDefault(m => m.Id == id);
        }

        public bool Add(Order.Api.Entity.Order order)
        {
            Orders.Add(order);
            return true;
        }

        public bool Save(Order.Api.Entity.Order order)
        {
            var currentOrder = Orders.FirstOrDefault(m => m.Id == order.Id);
            if (currentOrder == null)
            {
                return false;
            }
            currentOrder.Price = order.Price;
            return true;
        }
    }
}
