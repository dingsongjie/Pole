using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using System.Reflection;
using Pole.Application.Cqrs;
using Pole.Application.Cqrs.Internal;
using Pole.Application.Command;
using Pole.Application;
using Pole.Domain.UnitOfWork;
using Pole.Application.EventBus;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPole(this IServiceCollection services, Action<PoleOptions> config)
        {
            PoleOptions poleOptions = new PoleOptions(services);
            config(poleOptions);

            services.AddScoped<ICommandBus, DefaultCommandBus>();
            services.AddScoped<IUnitOfWork, DefaultUnitOfWork>();
            services.AddScoped<IWorker, ReliableMessageTransactionWorker>();
            services.AddScoped<IEventBus, ReliableEventBus>();

            services.AddScoped<IReliableMessageScopedBuffer, DefaultReliableMessageScopedBuffer>();

            return services;
        }
    }
}
