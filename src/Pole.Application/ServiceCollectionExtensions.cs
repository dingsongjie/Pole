using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using System.Reflection;
using Pole.Application.Cqrs;
using Pole.Application.Cqrs.Internal;

namespace Pole.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPoleApplication(this IServiceCollection services, Options options)
        {
            services.AddScoped<ICommandBus, DefaultCommandBus>();

            return services;
        }
    }
}
