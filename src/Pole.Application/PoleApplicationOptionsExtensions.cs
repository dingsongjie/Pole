using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pole.Application.Cqrs;
using Pole.Application.MediatR;
using Pole.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pole.Application
{
    public static class PoleApplicationOptionsExtensions
    {
        public static PoleApplicationOptions AutoInjectionDependency(this PoleApplicationOptions options)
        {
            var assemblies = options.ApplicationAssemblies;

            foreach (var assembly in assemblies)
            {
                AddScoped(options, assembly);
                AddTransient(options, assembly);
                AddSingleton(options, assembly);
            }
            return options;
        }

        public static PoleApplicationOptions AutoInjectionCommandHandlersAndDomainEventHandlers(this PoleApplicationOptions options, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            options.Services.AddMediatR(config =>
            {
                config.AddServiceLifetime(lifetime);
            }, options.ApplicationAssemblies.ToArray());
            return options;
        }

        private static void AddScoped(PoleApplicationOptions options, Assembly assembly)
        {
            var queriesImplements = assembly.GetTypes().Where(m => typeof(IScopedDenpendency).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract);
            foreach (var queriesImplement in queriesImplements)
            {
                var queriesService = queriesImplement.GetInterfaces().FirstOrDefault();
                options.Services.AddScoped(queriesService, queriesImplement);
            }
        }

        private static void AddTransient(PoleApplicationOptions options, Assembly assembly)
        {
            var queriesImplements = assembly.GetTypes().Where(m => typeof(ITransientDependency).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract);
            foreach (var queriesImplement in queriesImplements)
            {
                var queriesService = queriesImplement.GetInterfaces().FirstOrDefault();
                options.Services.AddTransient(queriesService, queriesImplement);
            }
        }

        private static void AddSingleton(PoleApplicationOptions options, Assembly assembly)
        {
            var queriesImplements = assembly.GetTypes().Where(m => typeof(ISingleDependency).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract);
            foreach (var queriesImplement in queriesImplements)
            {
                var queriesService = queriesImplement.GetInterfaces().FirstOrDefault();
                options.Services.AddSingleton(queriesService, queriesImplement);
            }
        }

    }
}
