using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IComteckReliableMessageBootstrap
    {
        Task Initialize(IServiceCollection services, List<Assembly> eventHandlerAssemblies, List<Assembly> eventAssemblies);
    }
}
