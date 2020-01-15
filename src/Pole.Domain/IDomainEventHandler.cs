using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Domain
{
    public interface IDomainEventHandler<TCommand> : INotificationHandler<TCommand> where TCommand : IDomainEvent
    {

    }
}
