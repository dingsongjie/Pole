using Backet.Api.Grains.Abstraction;
using Backet.Api.Infrastructure;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Backet.Api.Domain.Event;
using Pole.Core.UnitOfWork;
using Pole.Core.EventBus.Transaction;
using Pole.Core.EventBus;

namespace Backet.Api.Grains
{
    public class AddBacketGrain : Grain, IAddBacketGrain
    {
        public async Task<bool> AddBacket(BacketDto backetDto)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var dbContext = scope.ServiceProvider.GetRequiredService<BacketDbContext>();
                var bus = scope.ServiceProvider.GetRequiredService<IBus>();
                using (var transaction = await dbContext.Database.BeginTransactionAsync())
                {
                    unitOfWork.Enlist(transaction, bus);
                    Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet backet = new Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet
                    {
                        Id = backetDto.Id,
                        UserId = backetDto.UserId
                    };
                    if (backetDto.BacketItems == null || backetDto.BacketItems.Count == 0) return false;
                    backetDto.BacketItems.ForEach(item =>
                    {
                        backet.AddBacketItem(item.ProductId, item.ProductName, item.Price);
                    });
                    dbContext.Backets.Add(backet);
                    await bus.Publish(new BacketCreatedEvent() { BacketId = backet.Id });
                    await unitOfWork.CompeleteAsync();
                }
                return true;
            }
        }
    }
}
