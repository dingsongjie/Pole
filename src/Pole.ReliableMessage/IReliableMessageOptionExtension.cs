using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage
{
    public interface IReliableMessageOptionExtension
    {
        void AddServices(IServiceCollection services);
    }
}
