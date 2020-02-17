using Orleans;
using Product.Api.Domain.AggregatesModel.ProductTypeAggregate;
using Product.Api.Grains.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Grains
{
    public class ProductTypeGrain : Grain<ProductType>, IProductTypeGrain
    {
        public async Task<bool> AddProductType(string id, string name)
        {
            if (State != null) return false;

            ProductType productType = new ProductType
            {
                Id = id,
                Name = name
            };
            State = productType;
            await WriteStateAsync();
            return true;
        }
    }
}
