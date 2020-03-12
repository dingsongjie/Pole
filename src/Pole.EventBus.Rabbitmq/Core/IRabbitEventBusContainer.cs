using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Pole.EventBus.RabbitMQ
{
    public interface IRabbitEventBusContainer : IConsumerContainer
    {
        Task AutoRegister();
        RabbitEventBus CreateEventBus(string exchange, string routePrefix, int lBCount = 1, bool reenqueue = false, bool persistent = false);
        Task Work(RabbitEventBus bus);
    }
}
