using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.IntegrationEvents
{
    public class ProductAddedIntegrationEvent
    {
        public string BacketId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public long Price { get; set; }
    }
}
