using Pole.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Domain.ProductAggregate
{
    public class Product : Entity, IAggregateRoot
    {
        public Product(string id ,string name,long price, string productTypeId)
        {
            Id = id;
            Name = name;
            Price = price;
            ProductTypeId = productTypeId;
        }
        public string Name { get;private set; }
        public long Price { get;private set; }
        public string ProductTypeId { get;private set; }
    }
}
