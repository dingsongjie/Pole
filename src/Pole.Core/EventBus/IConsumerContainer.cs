using System.Collections.Generic;

namespace Pole.Core.EventBus
{
    public interface IConsumerContainer
    {
        List<IConsumer> GetConsumers();
    }
}
