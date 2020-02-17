using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Grains.Abstraction
{
    public interface IProductTypeGrain: IGrainWithStringKey
    {
        Task<bool> AddProductType(string id, string name);
    }
}
