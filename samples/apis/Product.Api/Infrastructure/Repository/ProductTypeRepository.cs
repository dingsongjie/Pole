using Pole.Domain.UnitOfWork;
using Product.Api.Domain.ProductTypeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Infrastructure.Repository
{
    public class ProductTypeRepository : IProductTypeRepository
    {
        private readonly ProductDbContext _productDbContext;

        public ProductTypeRepository(ProductDbContext productDbContext)
        {
            _productDbContext = productDbContext;
        }
        public IUnitOfWork UnitOfWork => _productDbContext;

        public void Add(ProductType entity)
        {
            _productDbContext.ProductTypes.Add(entity);
        }

        public void Delete(ProductType entity)
        {
            throw new NotImplementedException();
        }

        public Task<ProductType> Get(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(ProductType entity)
        {
            throw new NotImplementedException();
        }
    }
}
