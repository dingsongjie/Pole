using Pole.Application.Command;
using Pole.Application.Cqrs;
using Pole.Grpc.ExtraType;
using PoleSample.Apis.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Product.Api.Application.Command
{
    public class AddProductTypeCommand: ICommand<CommonCommandResponse>
    {
        public AddProductTypeRequest Request { get; set; }
    }
}
