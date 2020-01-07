using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Application.Cqrs
{
    public interface ICommandHandler<TCommand,TResult>: IRequestHandler<TCommand, TResult> where TCommand : IRequest<TResult>
    {
    }
}
