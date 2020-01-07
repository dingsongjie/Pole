using Pole.ReliableMessage.EventBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Masstransit.Abstraction
{
    public interface IReliableEventHandlerRegistrarFactory
    {
        MasstransitEventHandlerRegistrar Create(Type eventHandlerType);
    }
}
