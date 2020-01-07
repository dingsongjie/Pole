using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IMessageBusConfigurator
    {
        Task Configure(IServiceCollection services, IEnumerable<Type> eventHandlerTypes);
    }
}
