using Pole.Domain.EntityframeworkCore;
using Pole.Domain.UnitOfWork;
using Product.Api.Domain.ProductAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Infrastructure.Repository
{
    public class ProductRepository : EFCoreRepository<Product.Api.Domain.ProductAggregate.Product>, IProductRepository
    {
        public ProductRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
    }
}
