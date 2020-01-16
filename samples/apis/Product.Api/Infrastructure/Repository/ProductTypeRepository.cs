using Pole.Domain.EntityframeworkCore;
using Pole.Domain.UnitOfWork;
using Product.Api.Domain.ProductTypeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Infrastructure.Repository
{
    public class ProductTypeRepository : EFCoreRepository<ProductType>, IProductTypeRepository
    {
        public ProductTypeRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
