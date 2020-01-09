using MediatR;
using MediatR.Registration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMediatR(this IServiceCollection services)
        {
            var serviceConfig = new MediatRServiceConfiguration();

            ServiceRegistrar.AddRequiredServices(services, serviceConfig);

            ServiceRegistrar.AddMediatRClasses(services, new Assembly[0]);

            return services;
        }
    }
}
