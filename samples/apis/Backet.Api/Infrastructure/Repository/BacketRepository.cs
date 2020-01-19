using Backet.Api.Domain.AggregatesModel.BacketAggregate;
using Pole.Domain.EntityframeworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Backet.Api.Infrastructure.Repository
{
    public class BacketRepository : EFCoreRepository<Backet.Api.Domain.AggregatesModel.BacketAggregate.Backet>, IBacketRepository
    {
        public BacketRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public override async Task<Domain.AggregatesModel.BacketAggregate.Backet> Get(string id)
        {
            var backet = await base.Get(id);
            if (backet != null)
            {
                await _dbContext.Entry(backet).Collection(m => m.BacketItems).LoadAsync();
            }
            return backet;
        }
    }
}
