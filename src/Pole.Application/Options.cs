using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Pole.Application
{
    public class Options
    {
        public IServiceCollection Services { get; set; }
        public IEnumerable<Assembly> ApplicationAssemblies { get; set; }

    }
}
