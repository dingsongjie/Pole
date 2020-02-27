using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pole.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Core.Query
{
    class QueryRegister : IQueryRegister
    {
        private readonly IServiceProvider serviceProvider;
        public QueryRegister(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public Task Register(IServiceCollection serviceCollection, ServiceLifetime serviceLifetime)
        {
            foreach (var assembly in AssemblyHelper.GetAssemblies(serviceProvider.GetService<ILogger<QueryRegister>>()))
            {
                var implements = assembly.GetTypes().Where(m => typeof(IQueries).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract);
                foreach (var implement in implements)
                {
                    var services = implement.GetInterfaces();
                    foreach (var queriesService in services)
                    {
                        if (serviceLifetime == ServiceLifetime.Scoped)
                        {
                            serviceCollection.AddScoped(queriesService, implement);
                        }
                        else if (serviceLifetime == ServiceLifetime.Singleton)
                        {
                            serviceCollection.AddSingleton(queriesService, implement);
                        }
                        else
                        {
                            serviceCollection.AddTransient(queriesService, implement);
                        }                     
                    }
                }
            }
            return Task.CompletedTask;              
        }
    }
}
