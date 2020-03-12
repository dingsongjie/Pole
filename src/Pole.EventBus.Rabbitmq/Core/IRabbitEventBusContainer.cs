using System.Threading.Tasks;

namespace Pole.EventBus.RabbitMQ
{
    public interface IRabbitEventBusContainer : IConsumerContainer
    {
        bool IsAutoRegisterFinished { get; }
        Task AutoRegister();
        RabbitEventBus CreateEventBus(string exchange, string routePrefix, int lBCount = 1, bool autoAck = false, bool reenqueue = false, bool persistent = false);
        Task Work(RabbitEventBus bus);
    }
}
