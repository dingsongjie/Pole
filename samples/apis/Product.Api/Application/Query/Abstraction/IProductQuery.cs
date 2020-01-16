using Pole.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Application.Query.Abstraction
{
    public interface IProductQuery: IScopedDenpendency
    {
        Task<Product.Api.Domain.ProductAggregate.Product> GetById(string id);
    }
}
