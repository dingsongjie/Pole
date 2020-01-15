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
        public static implicit operator CommonCommandResponse(CompleteResult domainHandleResult)
        {
            return new CompleteResult(domainHandleResult.Status, domainHandleResult.Message);
        }
    }
}
