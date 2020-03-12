using Microsoft.EntityFrameworkCore;
using Pole.Core;
using Pole.EventBus.EventStorage;
using Pole.EventBus.Transaction;
using Pole.EventStorage.PostgreSql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
   public static class PolePostgreSqlStartupConfigExtensions
    {
        public static StartupConfig AddEventBusEFCoreStorage<TContext>(this StartupConfig config)
   where TContext : DbContext
        {
            return config.AddEventBusEFCoreStorage<TContext>(opt => { });
        }
        public static StartupConfig AddEventBusEFCoreStorage<TContext>(this StartupConfig config, Action<EFOptions> configure)
            where TContext : DbContext
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));
            EFOptions eFOptions = new EFOptions();
            configure(eFOptions);
            Action<PostgreSqlOptions> postgreSqlOptionsConfig = postgreSqlOptions =>
            {
                postgreSqlOptions.DbContextType = typeof(TContext);
                postgreSqlOptions.Schema = eFOptions.Schema;
                using var scope = config.Services.BuildServiceProvider().CreateScope();
                var provider = scope.ServiceProvider;
                using var dbContext = (DbContext)provider.GetRequiredService(typeof(TContext));
                postgreSqlOptions.ConnectionString = dbContext.Database.GetDbConnection().ConnectionString;
            };
            config.Services.Configure(postgreSqlOptionsConfig);

            config.Services.AddScoped<IDbTransactionAdapter, PostgreSqlDbTransactionAdapter>();
            config.Services.AddSingleton<IEventStorage, PostgreSqlEventStorage>();
            config.Services.AddSingleton<IEventStorageInitializer, PostgreSqlEventStorageInitializer>();
            return config;
        }
    }
}
