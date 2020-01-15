using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using System.Reflection;
using Pole.Application.Cqrs;
using Pole.Application.Cqrs.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Pole.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPoleApplication(this IServiceCollection services, Action<PoleApplicationOptions> config, params Assembly[] assemblies)
        {
            PoleApplicationOptions poleApplicationOptions = new PoleApplicationOptions(services, assemblies);

            config(poleApplicationOptions);

            services.AddScoped<ICommandBus, DefaultCommandBus>();

            return services;
        }
    }
}
