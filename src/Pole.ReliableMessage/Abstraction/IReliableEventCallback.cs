using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IReliableEventCallback<TReliableEvent,TCallbackParemeter> : IReliableEventCallback
    {
        Task<bool> Callback(TCallbackParemeter callbackParemeter);
    }
    public interface IReliableEventCallback
    {

    }
}
