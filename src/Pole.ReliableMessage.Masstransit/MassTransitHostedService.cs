using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Masstransit
{
    public class MassTransitHostedService : IHostedService
    {
        readonly IBusControl _bus;
        readonly ILogger _logger;
        public MassTransitHostedService(IBusControl bus, ILogger<MassTransitHostedService> logger)
        {
            _bus = bus;
            _logger = logger;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _bus.StartAsync();
            _logger.LogInformation("MassTransit Bus Start Successful");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _bus.StopAsync();
            _logger.LogInformation("MassTransit Bus Stop Successful");
        }
    }
}
