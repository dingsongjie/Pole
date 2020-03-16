using Microsoft.Extensions.DependencyInjection;
using Pole.Core;
using Pole.Core.Processor;
using Pole.Core.Query;
using Pole.Core.Serialization;
using Pole.Core.Utils;
using Pole.Core.Utils.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PoleServiceCollectionExtensions
    {
        public static IServiceCollection AddPole(this IServiceCollection services, Action<StartupConfig> config)
        {
            StartupConfig startupOption = new StartupConfig(services);
            if (startupOption.PoleOptionsConfig == null)
            {
                services.Configure<PoleOptions>(option => { });
            }
            services.AddSingleton<ISerializer, DefaultJsonSerializer>();
            services.AddSingleton<IGeneratorIdSolver, InstanceIPV4_16IdGeneratorIdSolver>();
            services.AddSingleton<IQueryRegister, QueryRegister>();
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var generatorIdSolver = serviceProvider.GetService<IGeneratorIdSolver>();
                services.AddSingleton(typeof(ISnowflakeIdGenerator), factory => new SnowflakeIdGenerator(new DateTime(2020, 1, 1), 16, generatorIdSolver.GetGeneratorId()));

                var queryRegister = serviceProvider.GetService<IQueryRegister>();
                queryRegister.Register(services, ServiceLifetime.Scoped);
            }
            config(startupOption);
            return services;
        }
    }
}
