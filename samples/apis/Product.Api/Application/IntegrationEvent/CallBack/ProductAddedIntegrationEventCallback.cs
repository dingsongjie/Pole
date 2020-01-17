using Pole.ReliableMessage.Abstraction;
using Product.Api.Application.Query.Abstraction;
using Product.Api.Domain.ProductAggregate;
using Product.IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Application.IntegrationEvent.CallBack
{
    public class ProductAddedIntegrationEventCallback : IReliableEventCallback<ProductAddedIntegrationEvent, string>
    {
        private readonly IProductQuery _productQuery;
        public ProductAddedIntegrationEventCallback(IProductQuery productQuery)
        {
            _productQuery = productQuery;
        }


        public async Task<bool> Callback(string callbackParemeter)
        {
            return await _productQuery.GetById(callbackParemeter) != null;
        }
    }
}
