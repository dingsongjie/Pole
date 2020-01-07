using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Application.MediatR
{
    public static class MediatRServiceConfigurationExtensions
    {
        public static MediatRServiceConfiguration AddServiceLifetime(this MediatRServiceConfiguration configuration, ServiceLifetime lifetime)
        {
            if (lifetime == ServiceLifetime.Scoped)
            {
                configuration.AsScoped();
            }
            else if (lifetime == ServiceLifetime.Singleton)
            {
                configuration.AsSingleton();
            }
            else
            {
                configuration.AsTransient();
            }
            return configuration;
        }
    }
}
