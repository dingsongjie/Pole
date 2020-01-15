using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.Processor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.ReliableMessage
{
    public class BackgroundServiceBasedProcessorServer : BackgroundService, IProcessorServer
    {
        private readonly IServiceProvider _serviceProvider;
        private Task _compositeTask;

        public BackgroundServiceBasedProcessorServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task Start(CancellationToken stoppingToken)
        {

            ProcessingContext processingContext = new ProcessingContext(stoppingToken);
            List<LoopProcessor> loopProcessors = new List<LoopProcessor>();
            var innerProcessors = _serviceProvider.GetServices<IProcessor>();
            var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
            var timeHelper = _serviceProvider.GetService<ITimeHelper>();
            foreach (var innerProcessor in innerProcessors)
            {
                LoopProcessor processor = new LoopProcessor(innerProcessor, loggerFactory, timeHelper);
                loopProcessors.Add(processor);
            }
            var tasks = loopProcessors.Select(p => p.Process(processingContext));

            _compositeTask = Task.WhenAll(tasks);
            await _compositeTask;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Start(stoppingToken);
        }
    }
}
