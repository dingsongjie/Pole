using System;

namespace Pole.Core.EventBus
{
    public interface IGrainID
    {
        Type EventHandlerType { get; }
    }
}
