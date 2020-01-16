using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Pole.Application;
using Pole.Application.Cqrs;
using Pole.Application.MediatR;
using Pole.Core.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PoleOptionsExtensions
    {
        public static PoleOptions AddManageredAssemblies(this PoleOptions options, params Assembly []  assemblies) 
        {
            options.ApplicationAssemblies = assemblies;
            return options;
        }
        public static PoleOptions AutoInjectionDependency(this PoleOptions options)
        {
            var assemblies = options.ApplicationAssemblies??throw new Exception("Cant't find ApplicationAssemblies,You must Run  PoleOptions.AddManageredAssemblies First");

            foreach (var assembly in assemblies)
            {
                AddScoped(options, assembly);
                AddTransient(options, assembly);
                AddSingleton(options, assembly);
            }
            return options;
        }

        public static PoleOptions AutoInjectionCommandHandlersAndDomainEventHandlers(this PoleOptions options, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var assemblies = options.ApplicationAssemblies ?? throw new Exception("Cant't find ApplicationAssemblies,You must Run  PoleOptions.AddManageredAssemblies First");
            options.Services.AddMediatR(config =>
            {              
                config.AddServiceLifetime(lifetime);
            }, assemblies.ToArray());
            return options;
        }

        private static void AddScoped(PoleOptions options, Assembly assembly)
        {
            var implements = assembly.GetTypes().Where(m => typeof(IScopedDenpendency).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract);
            foreach (var implement in implements)
            {
                var services = implement.GetInterfaces();
                foreach(var queriesService in services)
                {
                    options.Services.AddScoped(queriesService, implement);
                }
            }
        }

        private static void AddTransient(PoleOptions options, Assembly assembly)
        {
            var implements = assembly.GetTypes().Where(m => typeof(ITransientDependency).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract);
            foreach (var implement in implements)
            {
                var services = implement.GetInterfaces();
                foreach (var queriesService in services)
                {
                    options.Services.AddTransient(queriesService, implement);
                }
            }
        }

        private static void AddSingleton(PoleOptions options, Assembly assembly)
        {
            var implements = assembly.GetTypes().Where(m => typeof(ISingleDependency).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract);
            foreach (var implement in implements)
            {
                var services = implement.GetInterfaces();
                foreach (var queriesService in services)
                {
                    options.Services.AddSingleton(queriesService, implement);
                }
            }
        }
    }
}
