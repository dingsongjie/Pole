using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Pole.Application
{
    public class PoleOptions
    {
        public PoleOptions(IServiceCollection services)
        {
            Services = services;
        }
        public IServiceCollection Services { get;private set; }
        public IEnumerable<Assembly> ApplicationAssemblies { get; set; }

    }
}
