using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pole.Grpc
{
    public class PoleGrpcOptions
    {
        public PoleGrpcOptions(IServiceCollection services,params Assembly [] assemblies)
        {
            Services = services;
            ApplicationAssemblies = assemblies.ToList();
            AutoInject = true;
        }
        public IServiceCollection Services { get;private set; }
        public IEnumerable<Assembly> ApplicationAssemblies { get; set; }
        public bool AutoInject { get; set; }
    }
}
