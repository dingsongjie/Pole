using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IReliableBus
    {
        Task<string> PrePublish<TReliableEvent>(TReliableEvent @event,object callbackParemeter, CancellationToken cancellationToken = default);
        Task<bool> Publish<TReliableEvent>(TReliableEvent @event,string prePublishMessageId, CancellationToken cancellationToken=default);
       Task<bool> Cancel<TReliableEvent>(TReliableEvent @event, string prePublishMessageId, CancellationToken cancellationToken = default); 
  }
}
