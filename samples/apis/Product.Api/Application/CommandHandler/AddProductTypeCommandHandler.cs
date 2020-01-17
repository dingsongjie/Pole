using NewArchitectureLab.Apps.Product;
using Pole.Application.Command;
using Pole.Application.Cqrs;
using Pole.Domain.UnitOfWork;
using Pole.Grpc.ExtraType;
using PoleSample.Apis.Product;
using Product.Api.Domain.Event;
using Product.Api.Domain.ProductTypeAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Product.Api.Application.CommandHandler
{
    public class AddProductTypeCommandHandler : ICommandHandler<Command<AddProductTypeRequest, CommonCommandResponse>, CommonCommandResponse>
    {
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddProductTypeCommandHandler(IProductTypeRepository productTypeRepository, IUnitOfWork unitOfWork)
        {
            _productTypeRepository = productTypeRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<CommonCommandResponse> Handle(Command<AddProductTypeRequest, CommonCommandResponse> request, CancellationToken cancellationToken)
        {
            var productType = new Domain.ProductTypeAggregate.ProductType(request.Data.Id, request.Data.Name);

            _productTypeRepository.Add(productType);
            ProductTypeAddedDomainEvent productTypeAddedDomainEvent = new ProductTypeAddedDomainEvent
            {
                ProductTypeId = productType.Id,
                ProductTypeName = productType.Name
            };

            productType.AddDomainEvent(productTypeAddedDomainEvent);
            var result = await _productTypeRepository.SaveEntitiesAsync();

            await _unitOfWork.Compelete();
            return CommonCommandResponse.SuccessResponse;
        }
    }
}
