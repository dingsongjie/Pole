using Microsoft.Extensions.Logging;
using Pole.Sagas.Core;
using Pole.Sagas.Server.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Pole.Sagas.Server
{
    class SagasBuffer : ISagasBuffer
    {
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, BufferBlock<SagaEntity>> Sagas = new System.Collections.Concurrent.ConcurrentDictionary<string, BufferBlock<SagaEntity>>();
        private readonly ILogger logger;
        public SagasBuffer(ILogger<SagasBuffer> logger)
        {
            this.logger = logger;
        }
        public async Task<bool> AddSagas(IAsyncEnumerable<SagasGroupEntity> sagasGroupEntities)
        {
            await foreach (var sagasGroupEntity in sagasGroupEntities)
            {
                if (!Sagas.ContainsKey(sagasGroupEntity.ServiceName))
                {
                    var bufferBlock = new BufferBlock<SagaEntity>();
                    sagasGroupEntity.SagaEntities.ForEach(m =>
                    {
                        bufferBlock.SendAsync(m);
                    });

                    Sagas.TryAdd(sagasGroupEntity.ServiceName, bufferBlock);

                }
                else
                {
                    // 这里必然为true
                    Sagas.TryGetValue(sagasGroupEntity.ServiceName, out BufferBlock<SagaEntity> bufferBlock);
                    sagasGroupEntity.SagaEntities.ForEach(m =>
                    {
                        bufferBlock.SendAsync(m);
                    });
                }
            }
            return true;
        }
        public Task<bool> CanConsume(string serviceName)
        {
            if (Sagas.TryGetValue(serviceName, out BufferBlock<SagaEntity> bufferBlock))
            {
                return bufferBlock.OutputAvailableAsync();
            }
            else
            {
                var newBufferBlock = new BufferBlock<SagaEntity>();
                Sagas.TryAdd(serviceName, newBufferBlock);
                return newBufferBlock.OutputAvailableAsync();
            }
        }

        public Task<SagaEntity>  GetSagaAvailableAsync(string serviceName)
        {
            if (Sagas.TryGetValue(serviceName, out BufferBlock<SagaEntity> bufferBlock))
            {
                return bufferBlock.ReceiveAsync();
            }
            else
            {
                var newBufferBlock = new BufferBlock<SagaEntity>();
                Sagas.TryAdd(serviceName, newBufferBlock);
                return newBufferBlock.ReceiveAsync();
            }
        }
    }
}
