using Pole.Domain.UnitOfWork;
using Product.Api.Domain.ProductAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Infrastructure.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _productDbContext;

        public ProductRepository(ProductDbContext productDbContext)
        {
            _productDbContext = productDbContext;
        }

        public IUnitOfWork UnitOfWork => _productDbContext;

        public void Add(Domain.ProductAggregate.Product entity)
        {
            _productDbContext.Products.Add(entity);      
        }

        public void Delete(Domain.ProductAggregate.Product entity)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.ProductAggregate.Product> Get(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(Domain.ProductAggregate.Product entity)
        {
            throw new NotImplementedException();
        }
    }
}
