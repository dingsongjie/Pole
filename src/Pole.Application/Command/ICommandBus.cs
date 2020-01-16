using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Application.Command
{
    public interface ICommandBus
    {
        Task<TResult> Send<TResult>(IRequest<TResult> request, CancellationToken cancellationToken = default);
        Task<object> Send(object request, CancellationToken cancellationToken = default);
    }
}
