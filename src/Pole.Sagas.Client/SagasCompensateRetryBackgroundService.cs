using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pole.Core.Serialization;
using Pole.Core.Utils.Abstraction;
using Pole.Sagas.Client.Abstraction;
using Pole.Sagas.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Pole.Sagas.Server.Grpc.Saga;

namespace Pole.Sagas.Client
{
    public class SagasCompensateRetryBackgroundService : BackgroundService
    {
        private readonly PoleSagasOption options;
        private readonly SagaClient sagaClient;
        private readonly SagaRestorer sagaRestorer;
        private readonly IEventSender eventSender;
        private readonly ILogger logger;
        public SagasCompensateRetryBackgroundService(IOptions<PoleSagasOption> options, SagaClient sagaClient, IServiceProvider serviceProvider, IEventSender eventSender, ILogger<SagasCompensateRetryBackgroundService> logger)
        {
            this.options = options.Value;
            this.sagaClient = sagaClient;
            sagaRestorer = new SagaRestorer(serviceProvider.GetRequiredService<ISnowflakeIdGenerator>(), serviceProvider, serviceProvider.GetRequiredService<IEventSender>(), this.options, serviceProvider.GetRequiredService<ISerializer>(), serviceProvider.GetRequiredService<IActivityFinder>());
            this.eventSender = eventSender;
            this.logger = logger;
        }

        private async Task GrpcGetSagasCore(CancellationToken cancellationToken)
        {
            using (var stream = sagaClient.GetSagas(new Pole.Sagas.Server.Grpc.GetSagasRequest { Limit = options.PreSagasGrpcStreamingResponseLimitCount, ServiceName = options.ServiceName }))
            {
                while (await stream.ResponseStream.MoveNext(cancellationToken))
                {
                    if (stream.ResponseStream.Current.IsSuccess)
                    {
                        try
                        {
                            var sagas = stream.ResponseStream.Current.Sagas.Select(m =>
                            {
                                var result = new SagaEntity
                                {
                                    Id = m.Id,
                                };
                                result.ActivityEntities = m.Activities.Select(n => new ActivityEntity
                                {
                                    CompensateTimes = n.CompensateTimes,
                                    OvertimeCompensateTimes = n.ExecuteTimes,
                                    Id = n.Id,
                                    Name = n.Name,
                                    Order = n.Order,
                                    ParameterData = n.ParameterData,
                                    SagaId = n.SagaId,
                                    Status = n.Status
                                }).ToList();
                                return result;
                            }).ToList();
                            sagas.ForEach(async sagaEntity =>
                            {
                                var saga = sagaRestorer.CreateSaga(sagaEntity);
                                var compensateResult = await saga.CompensateWhenRetry();
                                if (compensateResult)
                                {
                                    var expiresAt = DateTime.UtcNow.AddSeconds(options.CompeletedSagaExpiredAfterSeconds);
                                    await eventSender.SagaEnded(sagaEntity.Id, expiresAt);
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Errors in NotEndedSagasCompensateRetryBackgroundService CompensateRetry");
                        }
                    }
                }
            }
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                try
                {
                    await GrpcGetSagasCore(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Errors in grpc get sagas");
                }
                finally
                {
                    await Task.Delay(options.GrpcConnectFailRetryIntervalSeconds * 1000);
                }
            }
        }
    }
}
