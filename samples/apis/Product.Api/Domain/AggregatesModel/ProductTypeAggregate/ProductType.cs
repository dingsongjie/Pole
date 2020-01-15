using Pole.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Domain.ProductTypeAggregate
{
    public class ProductType : Entity, IAggregateRoot
    {
        public ProductType(string id,string name)
        {
            Id = id;
            Name = name;
        }
        public string Name { get;private set; }
    }
}
