using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SagasTest.Api.Activities
{
    public class Transaction1ReturnFalseActivity : IActivity<Transaction1Dto>
    {
        private readonly IHttpClientFactory httpClientFactory;
        public Transaction1ReturnFalseActivity(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;

        }
        public async Task Compensate(Transaction1Dto data, CancellationToken cancellationToken)
        {
            var httpclient = httpClientFactory.CreateClient();
            httpclient.BaseAddress = new Uri("http://localhost:5000");
            var result = await httpclient.GetAsync("api/OutGoingMock/Transaction1RollBack");
        }

        public async Task<ActivityExecuteResult> Execute(Transaction1Dto data, CancellationToken cancellationToken)
        {
            var httpclient = httpClientFactory.CreateClient();
            httpclient.BaseAddress = new Uri("http://localhost:5000");
            var result = await httpclient.GetAsync("api/OutGoingMock/Transaction1Ok");
            return new ActivityExecuteResult
            {
                IsSuccess = false,
                Result = "库存不足"
            };
        }
    }
}
