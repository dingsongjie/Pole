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
    public class Transaction3ExceptionActivity : IActivity<Transaction3Dto>
    {
        public Task Compensate(Transaction3Dto data)
        {
            Console.WriteLine("Transaction3 Rollback");
            return Task.CompletedTask;
        }

        public Task<ActivityExecuteResult> Execute(Transaction3Dto data, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
