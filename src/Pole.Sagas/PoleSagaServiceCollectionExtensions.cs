using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Pole.Core;
using Pole.Core.Utils;
using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;
using Pole.Sagas.Core.Exceptions;

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
            var baseActivityType = typeof(IActivity<>);
            foreach (var assembly in AssemblyHelper.GetAssemblies())
            {

                foreach (var type in assembly.GetTypes().Where(m => m.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseActivityType) && m.IsClass && !m.IsAbstract))
                {
                    if (!type.FullName.EndsWith("Activity"))
                    {
                        throw new ActivityNameIrregularException(type);
                    }
                    startupOption.Services.AddScoped(type);
                }
            }
        }
    }
}
