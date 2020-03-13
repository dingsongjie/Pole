using Microsoft.Extensions.Hosting;
using Pole.Core.Processor;
using Pole.Sagas.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Pole.Sagas.Server.Processor
{
    public class BackgroundServiceBasedProcessorServer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private Task _compositeTask;

        public BackgroundServiceBasedProcessorServer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var eventStorageInitializer = _serviceProvider.GetService<ISagaStorageInitializer>();
            await eventStorageInitializer.InitializeAsync(cancellationToken);
            await base.StartAsync(cancellationToken);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ProcessingContext processingContext = new ProcessingContext(stoppingToken);
            List<LoopProcessor> loopProcessors = new List<LoopProcessor>();
            var innerProcessors = _serviceProvider.GetServices<IProcessor>();
            var loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
            foreach (var innerProcessor in innerProcessors)
            {
                LoopProcessor processor = new LoopProcessor(innerProcessor, loggerFactory);
                loopProcessors.Add(processor);
            }
            var tasks = loopProcessors.Select(p => p.Process(processingContext));

            _compositeTask = Task.WhenAll(tasks);
            await _compositeTask;
        }
    }
}
