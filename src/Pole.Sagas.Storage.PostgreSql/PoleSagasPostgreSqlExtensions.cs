using Microsoft.Extensions.DependencyInjection;
using Pole.Core;
using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;
using Pole.Sagas.Storage.PostgreSql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PoleSagasPostgreSqlExtensions
    {
        public static StartupConfig AddSagasServerPGStorage(this StartupConfig startupConfig, Action<PoleSagasStoragePostgreSqlOption> config)
        {
            startupConfig.Services.Configure(config);
            startupConfig.Services.AddSingleton<ISagaStorageInitializer, PostgreSqlSagaStorageInitializer>();
            startupConfig.Services.AddSingleton<ISagaStorage, PostgreSqlSagaStorage>();
            return startupConfig;
        }
    }
}
