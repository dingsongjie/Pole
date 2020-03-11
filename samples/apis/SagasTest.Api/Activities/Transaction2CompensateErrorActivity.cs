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
    public class Transaction2CompensateErrorActivity : IActivity<Transaction2Dto>
    {
        private readonly IHttpClientFactory httpClientFactory;
        public Transaction2CompensateErrorActivity(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public Task Compensate(Transaction2Dto data, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ActivityExecuteResult> Execute(Transaction2Dto data, CancellationToken cancellationToken)
        {
            var httpclient = httpClientFactory.CreateClient();
            httpclient.BaseAddress = new Uri("http://localhost:5000");
            var result = await httpclient.GetAsync("api/OutGoingMock/Transaction2Ok");
            return ActivityExecuteResult.Success;
        }
    }
}
