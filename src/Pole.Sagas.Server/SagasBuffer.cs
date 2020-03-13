using Microsoft.Extensions.Logging;
using Pole.Sagas.Core;
using Pole.Sagas.Server.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Sagas.Server
{
    class SagasBuffer : ISagasBuffer
    {
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
        private readonly Dictionary<string, List<SagaEntity>> Sagas = new Dictionary<string, List<SagaEntity>>();
        private readonly ILogger logger;
        public SagasBuffer(ILogger<SagasBuffer> logger)
        {
            this.logger = logger;
        }
        public async Task<bool> AddSagas(IAsyncEnumerable<SagasGroupEntity> sagasGroupEntities)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                await foreach (var sagasGroupEntity in sagasGroupEntities)
                {
                    if (!Sagas.ContainsKey(sagasGroupEntity.ServiceName))
                    {
                        Sagas.TryAdd(sagasGroupEntity.ServiceName, sagasGroupEntity.SagaEntities);
                    }
                    else
                    {
                        // 这里必然为true
                        Sagas.TryGetValue(sagasGroupEntity.ServiceName, out List<SagaEntity> sagaList);
                        sagaList.AddRange(sagasGroupEntity.SagaEntities);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task<IEnumerable<SagaEntity>> GetSagas(string serviceName, int limit)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                if (Sagas.TryGetValue(serviceName, out List<SagaEntity> sagaList))
                {
                    var result = sagaList.Take(limit).ToList();
                    sagaList.RemoveAll(m => result.Select(n => n.Id).Contains(m.Id));
                    Sagas[serviceName] = sagaList;
                    return result;
                }
                return Enumerable.Empty<SagaEntity>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
