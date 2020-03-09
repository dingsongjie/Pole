using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Storage.PostgreSql
{
    public class PostgreSqlSagaStorage : ISagaStorage
    {
        private readonly string sagaTableName;
        private readonly string activityTableName;
        private readonly PoleSagasStoragePostgreSqlOption  poleSagasStoragePostgreSqlOption;
        private readonly ISagaStorageInitializer  sagaStorageInitializer;
        public PostgreSqlSagaStorage(IOptions<PoleSagasStoragePostgreSqlOption> poleSagasStoragePostgreSqlOption, ISagaStorageInitializer sagaStorageInitializer)
        {
            this.poleSagasStoragePostgreSqlOption = poleSagasStoragePostgreSqlOption.Value;
            this.sagaStorageInitializer = sagaStorageInitializer;
            sagaTableName = sagaStorageInitializer.GetSagaTableName();
            activityTableName = sagaStorageInitializer.GetActivityTableName();
        }
        public async Task ActivityCompensateAborted(string activityId, string sagaId, string errors)
        {          
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                using(var tansaction = await connection.BeginTransactionAsync())
                {
                    var updateActivitySql =
$"UPDATE {activityTableName} SET \"Status\"=@Status,\"Errors\"=@Errors WHERE \"Id\" = @Id";
                    await connection.ExecuteAsync(updateActivitySql,  new
                    {
                        Id = activityId,
                        Errors= errors,
                        Status= nameof(ActivityStatus.CompensateAborted)
                    }, tansaction);

                    var updateSagaSql =
$"UPDATE {sagaTableName} SET \"Status\"=@Status,\"Errors\"=@Errors WHERE \"Id\" = @Id";
                    await connection.ExecuteAsync(updateSagaSql, new
                    {
                        Id = activityId,
                        Status = nameof(ActivityStatus.CompensateAborted)
                    }, tansaction);
                }

            }
        }

        public Task ActivityCompensated(string activityId)
        {
            throw new NotImplementedException();
        }

        public Task ActivityEnded(string activityId, byte[] resultData)
        {
            throw new NotImplementedException();
        }

        public Task ActivityExecuteAborted(string activityId, string errors)
        {
            throw new NotImplementedException();
        }

        public Task ActivityExecuteOvertime(string activityId, string sagaId, string errors)
        {
            throw new NotImplementedException();
        }

        public Task ActivityExecuteStarted(string activityId, string sagaId, int timeOutSeconds, byte[] ParameterData, int order, DateTime addTime)
        {
            throw new NotImplementedException();
        }

        public Task ActivityRetried(string activityId, string status, int retries, ActivityRetryType retryType)
        {
            throw new NotImplementedException();
        }

        public Task ActivityRevoked(string activityId)
        {
            throw new NotImplementedException();
        }

        public Task SagaEnded(string sagaId, DateTime ExpiresAt)
        {
            throw new NotImplementedException();
        }

        public Task SagaStarted(string sagaId, string serviceName, DateTime addTime)
        {
            throw new NotImplementedException();
        }
    }
}
