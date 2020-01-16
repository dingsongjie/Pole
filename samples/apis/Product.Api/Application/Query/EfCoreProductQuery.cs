using Microsoft.EntityFrameworkCore;
using Product.Api.Application.Query.Abstraction;
using Product.Api.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Application.Query
{
    public class EfCoreProductQuery : IProductQuery
    {
        private readonly ProductDbContext _productDbContext;

        public EfCoreProductQuery(ProductDbContext productDbContext)
        {
            _productDbContext = productDbContext;
        }

        public Task<Domain.ProductAggregate.Product> GetById(string id)
        {
            return _productDbContext.Products.FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
