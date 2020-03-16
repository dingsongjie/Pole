using Microsoft.Extensions.DependencyInjection;
using Pole.Core;
using Pole.Core.Channels;
using Pole.Core.Processor;
using Pole.Core.Utils;
using Pole.EventBus;
using Pole.EventBus.EventHandler;
using Pole.EventBus.Processor;
using Pole.EventBus.Processor.Server;
using Pole.EventBus.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PoleEventBusStartupConfigExtensions
    {
        public static StartupConfig AddEventBus(this StartupConfig startupOption, Action<PoleEventBusOption> config = null)
        {
            Action<PoleEventBusOption> defaultConfig = option => { };
            var finalConfig = config ?? defaultConfig;

            startupOption.Services.Configure(finalConfig);
            startupOption.Services.AddSingleton<IEventBuffer, EventBuffer>();
            startupOption.Services.AddScoped<IBus, Bus>();
            startupOption.Services.AddTransient(typeof(IChannel<>), typeof(Channel<>));
            startupOption.Services.AddSingleton<IObserverUnitContainer, ObserverUnitContainer>();
            startupOption.Services.AddSingleton<IProcessor, PendingMessageRetryProcessor>();
            startupOption.Services.AddSingleton<IProcessor, ExpiredEventsCollectorProcessor>();
            startupOption.Services.AddHostedService<BackgroundServiceBasedProcessorServer>();
            startupOption.Services.AddScoped<IUnitOfWork, Pole.EventBus.UnitOfWork.UnitOfWork>();
            startupOption.Services.AddSingleton<IEventTypeFinder, EventTypeFinder>();

            RegisterEventHandler(startupOption);
            return startupOption;
        }

        private static void RegisterEventHandler(StartupConfig startupOption)
        {
            foreach (var assembly in AssemblyHelper.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes().Where(m => typeof(IPoleEventHandler).IsAssignableFrom(m) && m.IsClass && !m.IsAbstract && !typeof(Orleans.Runtime.GrainReference).IsAssignableFrom(m)))
                {
                    startupOption.Services.AddScoped(type);
                }
            }
        }
    }
}
