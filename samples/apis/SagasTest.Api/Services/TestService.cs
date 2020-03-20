using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SagasTest.Api.Services
{
    public class TestService:Test.TestBase
    {
        public override Task<TestRequest> AddBacket(TestRequest request, ServerCallContext context)
        {
            return base.AddBacket(request, context);
        }
    }
}
