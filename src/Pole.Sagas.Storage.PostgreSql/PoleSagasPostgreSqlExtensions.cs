using Microsoft.Extensions.DependencyInjection;
using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Storage.PostgreSql
{
    public static class PoleSagasPostgreSqlExtensions
    {
        public static IServiceCollection AddPostgreSqlStorage(IServiceCollection services,Action<PoleSagasStoragePostgreSqlOption> config)
        {
            services.Configure(config);
            services.AddSingleton<ISagaStorageInitializer, PostgreSqlEventStorageInitializer>();
            services.AddSingleton<ISagaStorage, PostgreSqlSagaStorage>();
            return services;
        }
    }
}
