using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Application.Command
{
    public interface ICommandHandler<TCommand,TResult>: IRequestHandler<TCommand, TResult> where TCommand : ICommand<TResult>
    {
    }
}
