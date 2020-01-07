using ServiceA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Api.Events
{
    public class CreateOrderReliableEvent 
    {
        public decimal OrderPrice { get; set; }
    }
}
