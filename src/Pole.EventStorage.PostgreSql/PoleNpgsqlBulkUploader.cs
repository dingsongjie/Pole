using Npgsql;
using NpgsqlTypes;
using Pole.Core.EventBus.EventStorage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Pole.EventStorage.PostgreSql
{
    public class PoleNpgsqlBulkUploader
    {
        private readonly NpgsqlConnection npgsqlConnection;
        private static int tablesCounter = 0;
        private static string uniqueTablePrefix = Guid.NewGuid().ToString().Replace("-", "_");

        public PoleNpgsqlBulkUploader(NpgsqlConnection npgsqlConnection)
        {
            this.npgsqlConnection = npgsqlConnection;
        }
        public async Task UpdateAsync(string tableName, IEnumerable<EventEntity> eventEntities)
        {
            await npgsqlConnection.OpenAsync();
            using (var transaction = await npgsqlConnection.BeginTransactionAsync())
            {
                var tempTableName = GetUniqueName("_temp_");

                // 1. Create temp table 
                var sql = $"CREATE TEMP TABLE {tempTableName} ON COMMIT DROP AS SELECT \"Retries\" , \"ExpiresAt\" , \"StatusName\" , \"Id\" FROM {tableName} LIMIT 0";
                await npgsqlConnection.ExecuteAsync(sql);

                // 2. Import into temp table
                using (var importer = npgsqlConnection.BeginBinaryImport($"COPY {tempTableName} (\"Retries\" , \"ExpiresAt\" , \"StatusName\" , \"Id\") FROM STDIN (FORMAT BINARY)"))
                {
                    foreach (var item in eventEntities)
                    {
                        importer.StartRow();
                        importer.Write(item.Retries);
                        if (item.ExpiresAt.HasValue)
                        {
                            importer.Write(item.ExpiresAt.Value, NpgsqlDbType.Timestamp);
                        }
                        else
                        {
                            importer.Write(DBNull.Value);
                        }

                        importer.Write(item.StatusName, NpgsqlDbType.Varchar);
                        importer.Write(item.Id, NpgsqlDbType.Varchar);
                    }
                    importer.Complete();
                }

                // 3. Insert into real table from temp one
                sql = $"UPDATE {tableName} target  SET \"Retries\" = \"source\".\"Retries\" , \"ExpiresAt\" = \"source\".\"ExpiresAt\" , \"StatusName\" = \"source\".\"StatusName\"  FROM {tempTableName} as source WHERE \"target\".\"Id\" = \"source\".\"Id\"";
                await npgsqlConnection.ExecuteAsync(sql);
                // 5. Commit
                transaction?.Commit();
            }
        }

        /// <summary>
        /// Get unique object name using user-defined prefix.
        /// </summary>
        /// <param name="prefix">Prefix.</param>
        /// <returns>Unique name.</returns>
        static string GetUniqueName(string prefix)
        {
            var counter = Interlocked.Increment(ref tablesCounter);
            return $"{prefix}_{uniqueTablePrefix}_{counter}";
        }
    }
}
