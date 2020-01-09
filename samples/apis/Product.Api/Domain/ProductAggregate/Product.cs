using Pole.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Domain.ProductAggregate
{
    public class Product : Entity, IAggregateRoot
    {
        public string Name { get; set; }
        public long Price { get; set; }
        public string ProductId { get; set; }
    }
}
