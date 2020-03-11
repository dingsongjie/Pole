using Pole.Sagas.Client.Abstraction;
using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SagasTest.Api.Activities
{
    public class Transaction3HasResultActivity : IActivity<Transaction3Dto>
    {
        public Task Compensate(Transaction3Dto data, CancellationToken cancellationToken)
        {
            Console.WriteLine("Transaction3 Rollback");
            return Task.CompletedTask;
        }

        public Task<ActivityExecuteResult> Execute(Transaction3Dto data, CancellationToken cancellationToken)
        {
            Console.WriteLine("Transaction3 commit");
            var result = new ActivityExecuteResult
            {
                IsSuccess = true,
                Result = new Transaction3DtoResult
                {
                    OrderId = 112,
                    UserName = "ccc"
                }
            };
            return Task.FromResult(result);
        }
    }
    public class Transaction3Dto
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
    public class Transaction3DtoResult
    {
        public int OrderId { get; set; }
        public string UserName { get; set; }
    }
}
