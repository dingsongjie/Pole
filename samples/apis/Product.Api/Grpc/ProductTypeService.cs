using Grpc.Core;
using NewArchitectureLab.Apps.Product;
using Pole.Application.Command;
using Pole.Application.Cqrs;
using Pole.Grpc.ExtraType;
using PoleSample.Apis.Product;
using Product.Api.Application.Command;
using Product.Api.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Grpc
{
    public class ProductTypeService: PoleSample.Apis.Product.ProductType.ProductTypeBase
    {
        private readonly ICommandBus _commandBus;
        public ProductTypeService(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }
        public override Task<CommonCommandResponse> Add(AddProductTypeRequest request, ServerCallContext context)
        {
            var cpmmand = new AddProductTypeCommand { Request = request };
            return _commandBus.Send(cpmmand);
        }
    }
}
