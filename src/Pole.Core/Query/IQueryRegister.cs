using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Core.Query
{
    public interface IQueryRegister
    {
        Task Register(IServiceCollection serviceCollection, ServiceLifetime serviceLifetime);
    }
}
