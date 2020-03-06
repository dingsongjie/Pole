using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SagasTest.Api.Activities
{
    public class Transaction3ReturnFalseActivity : IActivity<Transaction3Dto>
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
                IsSuccess = false,
                Result = "创建订单失败"
            };
            return Task.FromResult(result);
        }
    }
}
