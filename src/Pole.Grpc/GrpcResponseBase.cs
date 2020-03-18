using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Grpc
{
    public class GrpcResponseBase<TResponse>
    {
        public Status Status { get; set; } = Status.DefaultSuccess;
        public TResponse Response { get; set; }
    }
}
