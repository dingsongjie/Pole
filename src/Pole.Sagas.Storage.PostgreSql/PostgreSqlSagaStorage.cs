using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;
using Pole.Sagas.Server.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
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
$"UPDATE {activityTableName} SET \"Status\"=@Status,\"Errors\"=@Errors, \"CompensateTimes\"=\"CompensateTimes\"+1 WHERE \"Id\" = @Id";
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

        public async Task ActivityExecuteOvertime(string activityId, string name, byte[] parameterData, DateTime addTime)
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

        public async Task ActivityExecuting(string activityId, string activityName, string sagaId, byte[] ParameterData, int order, DateTime addTime)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                string sql =
           $"INSERT INTO {activityTableName} (\"Id\",\"Name\",\"SagaId\",\"Status\",\"ParameterData\",\"OvertimeCompensateTimes\",\"CompensateTimes\",\"AddTime\")" +
           $"VALUES(@Id,@Name,@SagaId,@Status,@ParameterData,@OvertimeCompensateTimes,@CompensateTimes,@AddTime);";
                _ = await connection.ExecuteAsync(sql, new
                {
                    Id = activityId,
                    Name = activityName,
                    SagaId = sagaId,
                    Status = nameof(ActivityStatus.Executing),
                    ExecutingOvertimeRetries = 0,
                    ParameterData = ParameterData,
                    OvertimeCompensateTimes = 0,
                    CompensateTimes = 0,
                    AddTime = addTime
                });
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
                    ExpiresAt = ExpiresAt,
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
                    ServiceName = serviceName,
                    Status = nameof(ActivityStatus.Revoked)
                });
            }
        }

        public async IAsyncEnumerable<SagasGroupEntity> GetSagas(DateTime dateTime, int limit)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                var updateActivitySql =
$"select limit_sagas.\"Id\" as SagaId,limit_sagas.\"ServiceName\",activities.\"Id\" as ActivityId,activities.\"Order\",activities.\"Status\",activities.\"ParameterData\",activities.\"ExecuteTimes\",activities.\"CompensateTimes\",activities.\"Name\" from \"Activities\" as activities  inner join(select \"Id\",\"ServiceName\" from \"Sagas\" where \"AddTime\" <= @AddTime and \"Status\" = '{nameof(SagaStatus.Started)}' limit @Limit ) as limit_sagas on activities.\"SagaId\" = limit_sagas.\"Id\" and activities.\"Status\" != @Status1 and activities.\"Status\" != @Status2";
                var activities = await connection.QueryAsync<ActivityAndSagaEntity>(updateActivitySql, new
                {
                    AddTime = dateTime,
                    Limit = limit,
                    Status1 = nameof(ActivityStatus.Compensated),
                    Status2 = nameof(ActivityStatus.Revoked)
                });
                var groupedByServiceNameActivities = activities.GroupBy(m => m.ServiceName);
                foreach (var groupedByServiceName in groupedByServiceNameActivities)
                {
                    SagasGroupEntity sagasGroupEntity = new SagasGroupEntity
                    {
                        ServiceName = groupedByServiceName.Key,
                    };
                    var groupedBySagaIds = groupedByServiceName.GroupBy(m => m.SagaId);
                    foreach (var groupedBySagaId in groupedBySagaIds)
                    {
                        SagaEntity sagaEntity = new SagaEntity
                        {
                            Id = groupedBySagaId.Key
                        };
                        foreach (var activity in groupedBySagaId)
                        {
                            ActivityEntity activityEntity = new ActivityEntity
                            {
                                CompensateTimes = activity.CompensateTimes,
                                ExecuteTimes = activity.ExecuteTimes,
                                Id = activity.Id,
                                Order = activity.Order,
                                ParameterData = activity.ParameterData,
                                SagaId = activity.SagaId,
                                Status = activity.Status,
                            };
                            sagaEntity.ActivityEntities.Add(activityEntity);
                        }
                        sagasGroupEntity.SagaEntities.Add(sagaEntity);
                    }
                    yield return sagasGroupEntity;
                }
            }
        }

        public Task<int> DeleteExpiredData(string tableName, DateTime ExpiredAt, int batchCount)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                var sql =
$"delete {tableName}   WHERE \"ExpiresAt\" < @ExpiredAt AND \"Id\" IN (SELECT \"Id\" FROM {tableName} LIMIT @BatchCount);";
                return connection.ExecuteAsync(sql, new
                {
                    ExpiredAt = ExpiredAt,
                    BatchCount = batchCount,
                });
            }
        }

        public async Task ActivityOvertimeCompensated(string activityId, bool Compensated)
        {
            using (var connection = new NpgsqlConnection(poleSagasStoragePostgreSqlOption.ConnectionString))
            {
                string updateActivitySql = string.Empty;
                if (!Compensated)
                {
                    updateActivitySql =
$"UPDATE {activityTableName} SET \"OvertimeCompensateTimes\"=\"OvertimeCompensateTimes\"+1 WHERE \"Id\" = @Id";
                    await connection.ExecuteAsync(updateActivitySql, new
                    {
                        Id = activityId
                    });
                }
                else
                {
                    updateActivitySql =
$"UPDATE {activityTableName} SET \"OvertimeCompensateTimes\"=\"OvertimeCompensateTimes\"+1 ,\"Status\"=@Status WHERE \"Id\" = @Id";
                    await connection.ExecuteAsync(updateActivitySql, new
                    {
                        Id = activityId,
                        Status = nameof(ActivityStatus.Compensated)
                    });
                }
            }
        }
    }
}
