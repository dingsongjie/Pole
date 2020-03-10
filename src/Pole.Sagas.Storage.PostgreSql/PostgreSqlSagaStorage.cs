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

        public async Task ActivityExecuted(string activityId)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                var updateActivitySql =
$"UPDATE {activityTableName} SET \"Status\"=@Status  WHERE \"Id\" = @Id";
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

        public async Task ActivityExecuting(string activityId, string activityName, string sagaId, byte[] ParameterData, int order, DateTime addTime, int executeTimes)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                string sql = string.Empty;
                if (executeTimes == 1)
                {
                    sql =
               $"INSERT INTO {activityTableName} (\"Id\",\"Name\",\"SagaId\",\"Status\",\"ParameterData\",\"ExecuteTimes\",\"CompensateTimes\",\"AddTime\")" +
               $"VALUES(@Id,@Name,@SagaId,@Status,@ParameterData,@ExecuteTimes,@CompensateTimes,@AddTime);";
                    _ = await connection.ExecuteAsync(sql, new
                    {
                        Id = activityId,
                        Name = activityName,
                        SagaId = sagaId,
                        Status = nameof(ActivityStatus.Executing),
                        ExecutingOvertimeRetries = 0,
                        ParameterData = ParameterData,
                        ExecuteTimes = executeTimes,
                        CompensateTimes = 0,
                        AddTime = addTime
                    });
                }
                else
                {
                    sql = $"UPDATE {activityTableName} SET \"ExecuteTimes\"=@ExecuteTimes  WHERE \"Id\" = @Id";
                    await connection.ExecuteAsync(sql, new
                    {
                        Id = activityId,
                        ExecuteTimes = executeTimes
                    });
                }
            }
        }

        public async Task ActivityRevoked(string activityId)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                var updateActivitySql =
$"UPDATE {activityTableName} SET \"Status\"=@Status  WHERE \"Id\" = @Id";
                await connection.ExecuteAsync(updateActivitySql, new
                {
                    Id = activityId,
                    Status = nameof(ActivityStatus.Revoked)
                });
            }
        }

        public async Task SagaEnded(string sagaId, DateTime ExpiresAt)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                var updateActivitySql =
$"UPDATE {sagaTableName} SET \"Status\"=@Status ,\"ExpiresAt\"=@ExpiresAt WHERE \"Id\" = @Id";
                await connection.ExecuteAsync(updateActivitySql, new
                {
                    Id = sagaId,
                    ExpiresAt= ExpiresAt,
                    Status = nameof(ActivityStatus.Revoked)
                });
            }
        }

        public async Task SagaStarted(string sagaId, string serviceName, DateTime addTime)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                var sql =
$"INSERT INTO {sagaTableName} (\"Id\",\"ServiceName\",\"Status\",\"AddTime\")" +
               $"VALUES(@Id,@ServiceName,@Status,@AddTime);";
                await connection.ExecuteAsync(sql, new
                {
                    Id = sagaId,
                    AddTime = addTime,
                    ServiceName=serviceName,
                    Status = nameof(ActivityStatus.Revoked)
                });
            }
        }

        public async Task ActivityCompensating(string activityId, int compensateTimes)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                var updateActivitySql =
$"UPDATE {activityTableName} SET \"Status\"=@Status ,\"CompensateTimes\"=@CompensateTimes WHERE \"Id\" = @Id";
                await connection.ExecuteAsync(updateActivitySql, new
                {
                    Id = activityId,
                    Status = nameof(ActivityStatus.Compensating),
                    CompensateTimes= compensateTimes,
                });
            }
        }
    }
}
