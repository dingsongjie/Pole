using Microsoft.Extensions.DependencyInjection;
using Pole.Core.Abstraction;
using Pole.Core.Channels;
using Pole.Core.EventBus;
using Pole.Core.Processor;
using Pole.Core.Processor.Server;
using Pole.Core.Serialization;
using Pole.Core.UnitOfWork;
using Pole.Core.Utils;
using Pole.Core.Utils.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddPole(this IServiceCollection services,Action<PoleOptions> config)
        {
            services.AddSingleton<IEventTypeFinder, EventTypeFinder>();
            services.AddTransient(typeof(IMpscChannel<>), typeof(MpscChannel<>));
            services.AddScoped<IBus, Bus>();
            services.AddScoped<IUnitOfWork, Pole.Core.UnitOfWork.UnitOfWork>();
            services.AddSingleton<ISerializer, DefaultJsonSerializer>();
            services.AddSingleton<IGeneratorIdSolver, InstanceIPV4_16IdGeneratorIdSolver>();
            services.AddSingleton<ISnowflakeIdGenerator, SnowflakeIdGenerator>();


            services.AddSingleton<IProcessor, PendingMessageRetryProcessor>();
            services.AddSingleton<IProcessor, ExpiredEventsCollectorProcessor>();
            services.AddHostedService<BackgroundServiceBasedProcessorServer>();
            return services;
        }
    }
}
