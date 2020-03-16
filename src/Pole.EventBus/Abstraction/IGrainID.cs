using System;

namespace Pole.EventBus.Abstraction
{
    public interface IGrainID
    {
        Type EventHandlerType { get; }
    }
}
