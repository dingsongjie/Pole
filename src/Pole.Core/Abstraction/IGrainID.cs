using System;

namespace Pole.Core.Abstractions
{
    public interface IGrainID
    {
        Type GrainType { get; }
    }
}
