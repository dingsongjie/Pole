using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Orleans.Provider.EntityframeworkCore
{
    public interface IGrainStateEntryConfigurator<TContext, TGrain, TEntity>
        where TContext : DbContext
        where TEntity : class
    {
        void ConfigureSaveEntry(ConfigureSaveEntryContext<TContext, TEntity> context);
    }
}

