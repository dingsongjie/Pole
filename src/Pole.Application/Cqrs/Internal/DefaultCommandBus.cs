using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Pole.Application.Cqrs.Internal
{
    class DefaultCommandBus : ICommandBus
    {
        private readonly IMediator _mediator;
        public DefaultCommandBus(IMediator mediator)
        {
            _mediator = mediator;
        }
        public Task<TResult> Send<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default)
        {
            return _mediator.Send(request, cancellationToken);
        }

        public Task<object> Send(object request, CancellationToken cancellationToken = default)
        {
            return _mediator.Send(request, cancellationToken);
        }
    }
}
