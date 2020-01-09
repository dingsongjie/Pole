using Pole.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Domain.ProductTypeAggregate
{
    public class ProductType : Entity, IAggregateRoot
    {
        public string Name { get; set; }
    }
}
