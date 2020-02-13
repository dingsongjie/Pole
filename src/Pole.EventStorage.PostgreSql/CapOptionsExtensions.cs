using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pole.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.EventStorage.PostgreSql
{
    public static class CapOptionsExtensions
    {
        public static PoleOptions UseEntityFrameworkEventStorage<TContext>(this PoleOptions options)
    where TContext : DbContext
        {
            return options.UseEntityFrameworkEventStorage<TContext>(opt => { });
        }
        public static PoleOptions UseEntityFrameworkEventStorage<TContext>(this PoleOptions options, Action<EFOptions> configure)
            where TContext : DbContext
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));
            EFOptions eFOptions = new EFOptions();
            configure(eFOptions);
            Action<PostgreSqlOptions> postgreSqlOptionsConfig = postgreSqlOptions =>
            {
                postgreSqlOptions.DbContextType = typeof(TContext);
                postgreSqlOptions.Schema = eFOptions.Schema;
                using var scope = options.Services.BuildServiceProvider().CreateScope();
                var provider = scope.ServiceProvider;
                using var dbContext = (DbContext)provider.GetRequiredService(typeof(TContext));
                postgreSqlOptions.ConnectionString = dbContext.Database.GetDbConnection().ConnectionString;
            };
            options.Services.Configure(postgreSqlOptionsConfig);

            return options;
        }
        public static PoleOptions UsePostgreSqlEventStorage<TContext>(this PoleOptions options, Action<PostgreSqlOptions> configure)
    where TContext : DbContext
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));
            options.Services.Configure(configure);
            return options;
        }
    }
}
