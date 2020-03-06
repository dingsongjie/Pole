using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SagasTest.Api.Activities
{
    public class Transaction1ExceptionActivity : IActivity<Transaction1Dto>
    {
        private readonly IHttpClientFactory httpClientFactory;
        public Transaction1ExceptionActivity(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;

        }
        public async Task Compensate(Transaction1Dto data)
        {
            var httpclient = httpClientFactory.CreateClient();
            httpclient.BaseAddress = new Uri("http://localhost:5000");
            var result = await httpclient.GetAsync("api/OutGoingMock/Transaction1RollBack");
        }

        public Task<ActivityExecuteResult> Execute(Transaction1Dto data)
        {
            throw new NotImplementedException();
        }
    }
}
