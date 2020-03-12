using System.Collections.Generic;

namespace Pole.EventBus
{
    public interface IConsumerContainer
    {
        List<IConsumer> GetConsumers();
    }
}
