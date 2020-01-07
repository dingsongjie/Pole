using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IProcessorServer
    {
        Task Start(CancellationToken stoppingToken);
    }
}
