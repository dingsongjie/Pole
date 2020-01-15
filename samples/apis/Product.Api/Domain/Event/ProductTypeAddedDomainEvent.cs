using Pole.Domain;
using PoleSample.Apis.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Domain.Event
{
    public class ProductTypeAddedDomainEvent: IDomainEvent
    {
        public string ProductTypeName { get; set; }
        public string ProductTypeId { get; set; }
    }
}
