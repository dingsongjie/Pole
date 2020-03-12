using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pole.Core;
using Pole.Core.Utils;
using Pole.Sagas.Client;
using Pole.Sagas.Client.Abstraction;
using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;
using Pole.Sagas.Core.Exceptions;
using static Pole.Sagas.Server.Grpc.Saga;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PoleSagaServiceCollectionExtensions
    {
        public static void AddSagas(this StartupConfig startupOption, Action<PoleSagasOption> configAction)
        {
            // 让客户端支持 没有TLS 的 grpc call
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            startupOption.Services.Configure(configAction);
            startupOption.Services.AddSingleton<IActivityFinder, ActivityFinder>();
            startupOption.Services.AddSingleton<IEventSender, EventSender>();
            startupOption.Services.AddSingleton<ISagaFactory, SagaFactory>();
            startupOption.Services.AddHostedService<NotEndedSagasCompensateRetryBackgroundService>();
            PoleSagasOption sagasOption = null;
            using (var provider = startupOption.Services.BuildServiceProvider())
            {
                sagasOption = provider.GetRequiredService<IOptions<PoleSagasOption>>().Value;
                startupOption.Services.AddGrpcClient<SagaClient>(o =>
                {
                    o.Address = new Uri(sagasOption.SagasServerHost);
                });
            }
            RegisterActivities(startupOption);
        }

        private static void RegisterActivities(StartupConfig startupOption)
        {
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
