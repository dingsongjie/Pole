using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPoleDomain(this IServiceCollection service)
        {
            service.AddMediatR();
            return service;
        }
    }
}
