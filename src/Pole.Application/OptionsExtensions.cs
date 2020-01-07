using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pole.Application.Cqrs;
using Pole.Application.MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pole.Application
{
    public static class OptionsExtensions
    {
        public static Options AutoInjectionQueries(this Options options)
        {
            var assemblies = options.ApplicationAssemblies;

            foreach (var assembly in assemblies)
            {
                var queriesImplements = assembly.GetTypes().Where(m => typeof(IQueries).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract);
                foreach (var queriesImplement in queriesImplements)
                {
                    var queriesService = queriesImplement.GetInterfaces().FirstOrDefault();
                    options.Services.AddScoped(queriesService, queriesImplement);
                }
            }
            return options;
        }
        public static Options AddApplicationAssemblies(this Options options, params Assembly[] assemblies)
        {
            options.ApplicationAssemblies = assemblies.AsEnumerable();
            return options;
        }
        public static Options AutoInjectionCommandHandlersAndDomainEventHandlers(this Options options, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            options.Services.AddMediatR(config =>
            {
                config.AddServiceLifetime(lifetime);
            }, options.ApplicationAssemblies.ToArray());
            return options;
        }
    }
}
