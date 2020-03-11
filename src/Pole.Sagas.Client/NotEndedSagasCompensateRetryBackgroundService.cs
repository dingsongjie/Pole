using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    public class NotEndedSagasCompensateRetryBackgroundService : IHostedService
    {
        private readonly PoleSagasOption options;
        private readonly SagaClient sagaClient;
        private readonly SagaRestorer sagaRestorer;
        public NotEndedSagasCompensateRetryBackgroundService(IOptions<PoleSagasOption> options, SagaClient sagaClient, IServiceProvider serviceProvider)
        {
            this.options = options.Value;
            this.sagaClient = sagaClient;
            sagaRestorer = new SagaRestorer(serviceProvider.GetRequiredService<ISnowflakeIdGenerator>(), serviceProvider, serviceProvider.GetRequiredService<IEventSender>(), this.options, serviceProvider.GetRequiredService<ISerializer>(), serviceProvider.GetRequiredService<IActivityFinder>());
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var stream = sagaClient.GetSagas(new Pole.Sagas.Server.Grpc.GetSagasRequest { Limit = options.PreSagasGrpcStreamingResponseLimitCount, ServiceName = options.ServiceName }))
            {
                while (await stream.ResponseStream.MoveNext(cancellationToken))
                {
                    if (stream.ResponseStream.Current.IsSuccess)
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
                                ExecuteTimes = n.ExecuteTimes,
                                Id = n.Id,
                                Name = n.Id,
                                Order = n.Order,
                                ParameterData = n.ParameterData.ToByteArray(),
                                SagaId = n.SagaId,
                                Status = n.Status
                            }).ToList();
                            return result;
                        }).ToList();
                        sagas.ForEach(async sagaEntity =>
                       {
                           var saga = sagaRestorer.CreateSaga(sagaEntity);
                           await saga.Compensate();
                       });
                    }
                }
                //await foreach (var getSagasResponse in stream.ResponseStream.ReadAllAsync(cancellationToken))
                //{
                //    if (getSagasResponse.IsSuccess)
                //    {
                //        var sagas = getSagasResponse.Sagas.Select(m =>
                //        {
                //            var result = new SagaEntity
                //            {
                //                Id = m.Id,
                //            };
                //            result.ActivityEntities = m.Activities.Select(n => new ActivityEntity
                //            {
                //                CompensateTimes = n.CompensateTimes,
                //                ExecuteTimes = n.ExecuteTimes,
                //                Id = n.Id,
                //                Name = n.Id,
                //                Order = n.Order,
                //                ParameterData = n.ParameterData.ToByteArray(),
                //                SagaId = n.SagaId,
                //                Status = n.Status
                //            }).ToList();
                //            return result;
                //        });

                //    }
                //}
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
