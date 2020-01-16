using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using System.Reflection;
using Pole.Application.Cqrs;
using Pole.Application.Cqrs.Internal;
using Pole.Application.Command;
using Pole.Application;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPole(this IServiceCollection services, Action<PoleOptions> config)
        {
            PoleOptions poleOptions = new PoleOptions(services);

            config(poleOptions);

            services.AddScoped<ICommandBus, DefaultCommandBus>();

            return services;
        }
    }
}
