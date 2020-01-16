using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pole.Domain.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Domain.EntityframeworkCore.UnitOfWork
{
    public class EntityFrameworkCoreUnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly DbContext _dbContext;
        public EntityFrameworkCoreUnitOfWorkManager(DbContextOptions dbContextOptions, IServiceProvider serviceProvider)
        {
            _dbContext = serviceProvider.GetRequiredService(dbContextOptions.ContextType) as DbContext;
        }
        public async Task<IUnitOfWork> BeginUnitOfWork()
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync();
            EntityFrameworkCoreUnitOfWork entityFrameworkCoreUnitOfWork = new EntityFrameworkCoreUnitOfWork(transaction);

            return entityFrameworkCoreUnitOfWork;
        }
    }
}
