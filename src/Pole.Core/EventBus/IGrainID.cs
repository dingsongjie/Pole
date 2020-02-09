using System;

namespace Pole.Core.EventBus
{
    public interface IGrainID
    {
        Type GrainType { get; }
    }
}
