using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Orleans.Provider.EntityframeworkCore
{
    public class DefaultGrainStateEntryConfigurator<TContext, TGrain, TEntity>
        : IGrainStateEntryConfigurator<TContext, TGrain, TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        public void ConfigureSaveEntry(ConfigureSaveEntryContext<TContext, TEntity> context)
        {
            if (context.IsPersisted)
            {
                // todo update necessary table
                //EntityEntry<TEntity> entry = context.DbContext.Set<TEntity>().Update(context.Entity);
            }
            else
            {
                EntityEntry<TEntity> entry = context.DbContext.Set<TEntity>().Add(context.Entity);
            }


            //entry.State = context.IsPersisted
            //    ? EntityState.Modified
            //    : EntityState.Added;
        }
    }
}
