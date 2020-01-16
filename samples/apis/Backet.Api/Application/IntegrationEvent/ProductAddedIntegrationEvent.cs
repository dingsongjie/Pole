using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.Application.IntegrationEvent
{
    public class ProductAddedIntegrationEvent
    {
        public string ProductName { get; set; }
        public long Price { get; set; }
    }
}
