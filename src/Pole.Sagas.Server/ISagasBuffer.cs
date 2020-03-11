using Pole.Sagas.Core;
using Pole.Sagas.Server.Grpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Server
{
    public interface ISagasBuffer
    {
        Task<IEnumerable<SagaEntity>> GetSagas(string serviceName, int limit);
        Task<bool> AddSagas(IAsyncEnumerable<SagasGroupEntity> sagasGroupEntities);
    }
}
