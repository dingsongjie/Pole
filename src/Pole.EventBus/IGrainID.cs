using System;

namespace Pole.EventBus
{
    public interface IGrainID
    {
        Type EventHandlerType { get; }
    }
}
