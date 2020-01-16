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

namespace Product.Api.Application.Command.CommandHandler
{
    public class AddProductTypeCommandHandler : ICommandHandler<AddProductTypeCommand, CommonCommandResponse>
    {
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public AddProductTypeCommandHandler(IProductTypeRepository productTypeRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _productTypeRepository = productTypeRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
        public async Task<CommonCommandResponse> Handle(AddProductTypeCommand request, CancellationToken cancellationToken)
        {
            var productType = new Domain.ProductTypeAggregate.ProductType(request.Request.Id, request.Request.Name);

            
             _productTypeRepository.Add(productType);
            ProductTypeAddedDomainEvent productTypeAddedDomainEvent = new ProductTypeAddedDomainEvent
            {
                ProductTypeId = productType.Id,
                ProductTypeName = productType.Name
            };
            using(var unitOfWork= await _unitOfWorkManager.BeginUnitOfWork())
            {
                productType.AddDomainEvent(productTypeAddedDomainEvent);
                var result = await _productTypeRepository.SaveEntitiesAsync();

                await unitOfWork.CompeleteAsync();
                return CommonCommandResponse.SuccessResponse;
            }
        }
    }
}
