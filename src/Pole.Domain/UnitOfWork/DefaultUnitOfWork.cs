using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Domain.UnitOfWork
{
    public class DefaultUnitOfWork : IUnitOfWork
    {
        private readonly List<IWorker> _workers;
        public DefaultUnitOfWork(IServiceProvider serviceProvider)
        {
            _workers = serviceProvider.GetServices<IWorker>().ToList();
        }
        public async Task Compelete(CancellationToken cancellationToken = default)
        {
            var preCommitTasks = _workers.OrderBy(worker => worker.Order).Select(async worker =>
            {
                await worker.PreCommit();
            });
            await Task.WhenAll(preCommitTasks);
            try
            {
                var commitTasks = _workers.OrderBy(worker => worker.Order).Select(async worker =>
                {
                    await worker.Commit();
                });
                await Task.WhenAll(commitTasks);
            }
            catch (Exception ex)
            {
                var rollbackTasks = _workers.OrderBy(worker => worker.Order).Where(worker => worker.WorkerStatus == WorkerStatus.Commited).Select(async worker =>
                 {
                     await worker.Rollback();
                 });
                await Task.WhenAll(rollbackTasks);
                throw ex;
            }
        }

        public void Dispose()
        {
            // Workers 都是 scoped 的 每次请求结束后 会自动 dispose 所以这里不需要 调用 Workers 的 dispose
            //_workers.OrderBy(worker => worker.Order).ToList().ForEach(m => m.Dispose());
        }

        public Task Rollback(CancellationToken cancellationToken = default)
        {
            _workers.OrderBy(worker => worker.Order).ToList().ForEach(m => m.Rollback());
            return Task.FromResult(1);
        }
    }
}
