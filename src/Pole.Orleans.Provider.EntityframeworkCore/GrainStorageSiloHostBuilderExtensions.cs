using Microsoft.EntityFrameworkCore;
using Orleans.Hosting;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Orleans.Provider.EntityframeworkCore
{
    public static class GrainStorageSiloHostBuilderExtensions
    {
        public static ISiloHostBuilder AddEfGrainStorageAsDefault<TContext>(this ISiloHostBuilder builder)
            where TContext : DbContext
        {
            return builder.AddEfGrainStorage<TContext>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME);
        }

        public static ISiloHostBuilder AddEfGrainStorage<TContext>(this ISiloHostBuilder builder,
            string providerName)
            where TContext : DbContext
        {
            return builder
                .ConfigureServices(services => { services.AddEfGrainStorage<TContext>(providerName); });
        }

        public static ISiloBuilder AddEfGrainStorageAsDefault<TContext>(this ISiloBuilder builder)
            where TContext : DbContext
        {
            return builder.AddEfGrainStorage<TContext>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME);
        }

        public static ISiloBuilder AddEfGrainStorage<TContext>(this ISiloBuilder builder,
            string providerName)
            where TContext : DbContext
        {
            return builder
                .ConfigureServices(services => { services.AddEfGrainStorage<TContext>(providerName); });
        }
    }
}
