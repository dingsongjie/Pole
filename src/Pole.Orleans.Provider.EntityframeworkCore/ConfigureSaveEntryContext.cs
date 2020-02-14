using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Orleans.Provider.EntityframeworkCore
{
    public class ConfigureSaveEntryContext<TContext, TEntity>
    {
        public ConfigureSaveEntryContext(TContext dbContext, TEntity entity)
        {
            DbContext = dbContext;
            Entity = entity;
        }

        public TContext DbContext { get; }

        public TEntity Entity { get; }

        public bool IsPersisted { get; set; }
    }
}
