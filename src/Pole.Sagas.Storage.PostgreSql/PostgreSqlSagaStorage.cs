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
        private readonly PoleSagasStoragePostgreSqlOption poleSagasStoragePostgreSqlOption;
        private readonly ISagaStorageInitializer sagaStorageInitializer;
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
                using (var tansaction = await connection.BeginTransactionAsync())
                {
                    var updateActivitySql =
$"UPDATE {activityTableName} SET \"Status\"=@Status,\"Errors\"=@Errors WHERE \"Id\" = @Id";
                    await connection.ExecuteAsync(updateActivitySql, new
                    {
                        Id = activityId,
                        Errors = errors,
                        Status = nameof(ActivityStatus.CompensateAborted)
                    }, tansaction);
                    if (!string.IsNullOrEmpty(sagaId))
                    {
                        var updateSagaSql =
$"UPDATE {sagaTableName} SET \"Status\"=@Status,\"Errors\"=@Errors WHERE \"Id\" = @Id";
                        await connection.ExecuteAsync(updateSagaSql, new
                        {
                            Id = sagaId,
                            Status = nameof(SagaStatus.Error)
                        }, tansaction);
                    }
                    await tansaction.CommitAsync();
                }

            }
        }

        public async Task ActivityCompensated(string activityId)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                var updateActivitySql =
$"UPDATE {activityTableName} SET \"Status\"=@Status WHERE \"Id\" = @Id";
                await connection.ExecuteAsync(updateActivitySql, new
                {
                    Id = activityId,
                    Status = nameof(ActivityStatus.Compensated)
                });
            }
        }

        public async Task ActivityExecuted(string activityId, byte[] resultData)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                var updateActivitySql =
$"UPDATE {activityTableName} SET \"Status\"=@Status \"ResultData\"=@ResultData WHERE \"Id\" = @Id";
                await connection.ExecuteAsync(updateActivitySql, new
                {
                    Id = activityId,
                    Status = nameof(ActivityStatus.Executed)
                });
            }
        }

        public async Task ActivityExecuteAborted(string activityId)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                var updateActivitySql =
$"UPDATE {activityTableName} SET \"Status\"=@Status  WHERE \"Id\" = @Id";
                await connection.ExecuteAsync(updateActivitySql, new
                {
                    Id = activityId,
                    Status = nameof(ActivityStatus.ExecuteAborted)
                });
            }
        }

        public async Task ActivityExecuteOvertime(string activityId)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                var updateActivitySql =
$"UPDATE {activityTableName} SET \"Status\"=@Status  WHERE \"Id\" = @Id";
                await connection.ExecuteAsync(updateActivitySql, new
                {
                    Id = activityId,
                    Status = nameof(ActivityStatus.ExecutingOvertime)
                });
            }
        }

        public Task ActivityExecuting(string activityId, string sagaId, byte[] ParameterData, int order, DateTime addTime)
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

        public Task ActivityCompensating(string activityId)
        {
            throw new NotImplementedException();
        }
    }
}
