using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IApplicationBuilderConfigurator
    {
        void Config(IApplicationBuilder applicationBuilder);
        void Add(Action<IApplicationBuilder> config);
    }
}
