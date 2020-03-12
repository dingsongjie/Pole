using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Pole.EventBus.EventStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.EventStorage.PostgreSql
{
    class PostgreSqlEventStorageInitializer : IEventStorageInitializer
    {
        private readonly PostgreSqlOptions options;
        private readonly ILogger logger;
        public PostgreSqlEventStorageInitializer(IOptions<PostgreSqlOptions> options,ILogger<PostgreSqlEventStorageInitializer> logger)
        {
            this.options = options.Value;
            this.logger = logger;
        }
        public string GetTableName()
        {
            return $"\"{options.Schema}\".\"{options.TableName}\"";
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var sql = CreateDbTablesScript(options.Schema);
            using (var connection = new NpgsqlConnection(options.ConnectionString))
            {
                await connection.ExecuteAsync(sql);
            }

            logger.LogDebug("Ensuring all create database tables script are applied.");
        }
        protected virtual string CreateDbTablesScript(string schema)
        {
            var batchSql = $@"
CREATE SCHEMA IF NOT EXISTS ""{schema}"";

CREATE TABLE IF NOT EXISTS {GetTableName()}(
	""Id"" VARCHAR(20) PRIMARY KEY NOT NULL,
    ""Version"" VARCHAR(20) NOT NULL,
	""Name"" VARCHAR(200) NOT NULL,
	""Content"" TEXT NULL,
	""Retries"" INT NOT NULL,
	""Added"" TIMESTAMP NOT NULL,
    ""ExpiresAt"" TIMESTAMP NULL,
	""StatusName"" VARCHAR(10) NOT NULL
);";
            return batchSql;
        }
    }
}
