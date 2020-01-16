using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Application.Command
{
    public interface ICommand<TResponse>:IRequest<TResponse>
    {
    }
}
