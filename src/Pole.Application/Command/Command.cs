using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Application.Command
{
    public class Command<TRequest,TResponse>:ICommand<TResponse>
    {
        public Command(TRequest request)
        {
            Data = request;
        }
       public TRequest Data { get; private set; }
    }
}
