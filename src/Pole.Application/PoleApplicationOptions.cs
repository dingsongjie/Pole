using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Pole.Application
{
    public class PoleApplicationOptions
    {
        public PoleApplicationOptions(IServiceCollection services, params Assembly [] applicationAssemblies)
        {
            Services = services;
            ApplicationAssemblies = applicationAssemblies;
        }
        public IServiceCollection Services { get; set; }
        public IEnumerable<Assembly> ApplicationAssemblies { get; set; }

    }
}
