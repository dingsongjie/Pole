using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pole.EntityframeworkCore.MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Domain.EntityframeworkCore
{
    public class EFCoreRepository<TEntity> : IRepository<TEntity>
        where TEntity : Entity, IAggregateRoot
    {
        protected readonly DbContext _dbContext;
        private readonly IMediator _mediator;
        public EFCoreRepository(IServiceProvider serviceProvider)
        {
            var dbContextOptions = serviceProvider.GetRequiredService<DbContextOptions>();
            _dbContext = serviceProvider.GetRequiredService(dbContextOptions.ContextType) as DbContext;
            _mediator = serviceProvider.GetRequiredService<IMediator>();
        }
        public void Add(TEntity entity)
        {
            _dbContext.Set<TEntity>().Add(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public virtual Task<TEntity> Get(string id)
        {
            return _dbContext.Set<TEntity>().FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await _mediator.DispatchDomainEventsAsync(_dbContext);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
