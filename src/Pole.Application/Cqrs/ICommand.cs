using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Application.Cqrs
{
    public interface ICommand<TResponse>:IRequest<TResponse>
    {
    }
}
