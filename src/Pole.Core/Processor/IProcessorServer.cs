using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Core.Processor
{
    public interface IProcessorServer
    {
        Task Start(CancellationToken stoppingToken);
    }
}
