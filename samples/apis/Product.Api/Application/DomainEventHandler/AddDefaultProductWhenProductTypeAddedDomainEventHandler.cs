using Pole.Domain;
using Product.Api.Domain.Event;
using Product.Api.Domain.ProductAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Product.Api.Application.DomainEventHandler
{
    public class AddDefaultProductWhenProductTypeAddedDomainEventHandler : IDomainEventHandler<ProductTypeAddedDomainEvent>
    {
        private readonly IProductRepository _productRepository;
        public AddDefaultProductWhenProductTypeAddedDomainEventHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<DomainHandleResult> Handle(ProductTypeAddedDomainEvent request, CancellationToken cancellationToken)
        {
            Product.Api.Domain.ProductAggregate.Product product = new Product.Api.Domain.ProductAggregate.Product(Guid.NewGuid().ToString("N"), request.ProductTypeName, 100, request.ProductTypeId);
            _productRepository.Add(product);
           return await _productRepository.UnitOfWork.CompeleteAsync();
             
        }
    }
}
