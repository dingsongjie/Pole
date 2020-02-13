﻿using Dapper;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Npgsql;
using Pole.Core;
using Pole.Core.EventBus.EventStorage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.EventStorage.PostgreSql
{
    class PostgreSqlEventStorage : IEventStorage
    {
        private readonly string tableName;
        private readonly ProducerOptions producerOptions;
        private readonly PostgreSqlOptions options;
        private readonly IEventStorageInitializer eventStorageInitializer;
        public PostgreSqlEventStorage(IOptions<PostgreSqlOptions> postgreSqlOptions, IOptions<ProducerOptions> producerOptions, IEventStorageInitializer eventStorageInitializer)
        {
            this.producerOptions = producerOptions.Value;
            this.options = postgreSqlOptions.Value;
            this.eventStorageInitializer = eventStorageInitializer;
            tableName = eventStorageInitializer.GetTableName();
        }
        public async Task ChangePublishStateAsync(EventEntity message, EventStatus state)
        {
            var sql =
    $"UPDATE {tableName} SET \"Retries\"=@Retries,\"ExpiresAt\"=@ExpiresAt,\"StatusName\"=@StatusName WHERE \"Id\"=@Id";
            using var connection = new NpgsqlConnection(options.ConnectionString);
            await connection.ExecuteAsync(sql, new
            {
                Id = long.Parse(message.Id),
                message.Retries,
                message.ExpiresAt,
                StatusName = state.ToString("G")
            });
        }

        public async Task<int> DeleteExpiresAsync(string table, DateTime timeout, int batchCount = 1000, CancellationToken token = default)
        {
            using var connection = new NpgsqlConnection(options.ConnectionString);

            return await connection.ExecuteAsync(
                $"DELETE FROM {table} WHERE \"ExpiresAt\" < @timeout AND \"Id\" IN (SELECT \"Id\" FROM {table} LIMIT @batchCount);",
                new { timeout, batchCount });
        }

        public async Task<IEnumerable<EventEntity>> GetPublishedMessagesOfNeedRetry()
        {
            var fourMinAgo = DateTime.Now.AddMinutes(-4).ToString("O");
            var sql =
                $"SELECT * FROM {tableName} WHERE \"Retries\"<{producerOptions.FailedRetryCount} AND \"Added\"<'{fourMinAgo}' AND (\"StatusName\"='{EventStatus.Failed}' OR \"StatusName\"='{EventStatus.PrePublish}') LIMIT 200;";

            var result = new List<EventEntity>();
            using var connection = new NpgsqlConnection(options.ConnectionString);
            var reader = await connection.ExecuteReaderAsync(sql);
            while (reader.Read())
            {
                result.Add(new EventEntity
                {
                    Id = reader.GetString(0),
                    Retries = reader.GetInt32(4),
                    Added = reader.GetDateTime(5)
                });
            }

            return result;
        }

        public async Task<bool> StoreMessage(EventEntity eventEntity, object dbTransaction = null)
        {
            var sql =
                $"INSERT INTO {tableName} (\"Id\",\"Version\",\"Name\",\"Content\",\"Retries\",\"Added\",\"ExpiresAt\",\"StatusName\")" +
                $"VALUES(@Id,'1',@Name,@Content,@Retries,@Added,@ExpiresAt,@StatusName);";

            if (dbTransaction == null)
            {
                using var connection = new NpgsqlConnection(options.ConnectionString);
                return await connection.ExecuteAsync(sql, eventEntity) > 0;
            }
            else
            {
                var dbTrans = dbTransaction as IDbTransaction;
                if (dbTrans == null && dbTransaction is IDbContextTransaction dbContextTrans)
                    dbTrans = dbContextTrans.GetDbTransaction();

                var conn = dbTrans?.Connection;
                return await conn.ExecuteAsync(sql, eventEntity, dbTrans) > 0;
            }
        }
    }
}
