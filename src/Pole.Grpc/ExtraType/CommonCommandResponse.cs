using Pole.Application.Cqrs;
using Pole.Domain;
using Pole.Domain.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Grpc.ExtraType
{
    public partial class CommonCommandResponse
    {
        public static CommonCommandResponse SuccessResponse = new CommonCommandResponse()
        {
            Message = "执行成功",
            Status = 1
        };
    }
}
