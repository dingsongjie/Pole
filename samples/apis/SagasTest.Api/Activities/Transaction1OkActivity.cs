using Pole.Sagas.Client.Abstraction;
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
    public class Transaction1OkActivity : IActivity<Transaction1Dto>
    {
        private readonly IHttpClientFactory httpClientFactory;
        public Transaction1OkActivity(IHttpClientFactory httpClientFactory)
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
            var result = await httpclient.GetAsync("api/OutGoingMock/Transaction1Ok", cancellationToken);
            return ActivityExecuteResult.Success;
        }
    }
    public class Transaction1Dto
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
