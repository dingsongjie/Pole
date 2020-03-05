using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pole.Core;
using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PoleSagaServiceCollectionExtensions
    {
        public static void AddSagas(this StartupConfig startupOption, Action<PoleSagasOption> rabbitConfigAction)
        {
            startupOption.Services.Configure(rabbitConfigAction);
            startupOption.Services.AddSingleton<IActivityFinder, ActivityFinder>();
            startupOption.Services.AddSingleton<IEventSender, EventSender>();
            startupOption.Services.AddSingleton<ISagaFactory, SagaFactory>();
        }
        public static void AddSagas(this StartupConfig startupOption)
        {
            Action<PoleSagasOption> action = option => { };
            startupOption.Services.Configure(action);
            startupOption.Services.AddSingleton<IActivityFinder, ActivityFinder>();
            startupOption.Services.AddSingleton<IEventSender, EventSender>();
            startupOption.Services.AddSingleton<ISagaFactory, SagaFactory>();
        }
    }
}
