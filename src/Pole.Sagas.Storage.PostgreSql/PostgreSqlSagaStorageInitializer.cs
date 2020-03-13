using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Pole.Sagas.Core;
using Pole.Sagas.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Sagas.Storage.PostgreSql
{
    class PostgreSqlSagaStorageInitializer : ISagaStorageInitializer
    {
        private PoleSagasStoragePostgreSqlOption options;
        private readonly ILogger logger;
        public PostgreSqlSagaStorageInitializer(IOptions<PoleSagasStoragePostgreSqlOption> poleSagaServerOption, ILogger<PostgreSqlSagaStorageInitializer> logger)
        {
            this.options = poleSagaServerOption.Value;
            this.logger = logger;
        }
        public string GetActivityTableName()
        {
            return $"\"{options.SchemaName}\".\"{options.ActivityTableName}\"";
        }

        public string GetSagaTableName()
        {
            return $"\"{options.SchemaName}\".\"{options.SagaTableName}\"";
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var sql = CreateDbTablesScript(options.SchemaName);

            using (var connection = new NpgsqlConnection(options.ConnectionString))
            {
                await connection.ExecuteAsync(sql);
            }
            logger.LogDebug("Ensuring all create database tables script are applied.");
        }

        private string CreateDbTablesScript(string schemaName)
        {
            var batchSql = $@"
CREATE SCHEMA IF NOT EXISTS ""{options.SchemaName}"";

CREATE TABLE IF NOT EXISTS {GetSagaTableName()}(
  ""Id"" varchar(20) COLLATE ""pg_catalog"".""default"" NOT NULL,
  ""ServiceName"" varchar(64) COLLATE ""pg_catalog"".""default"" NOT NULL,
  ""Status"" varchar(10) COLLATE ""pg_catalog"".""default"" NOT NULL,
  ""ExpiresAt"" timestamp,
  ""AddTime"" timestamp NOT NULL,
  CONSTRAINT ""Sagas_pkey"" PRIMARY KEY (""Id"")
);

CREATE TABLE IF NOT EXISTS {GetActivityTableName()}(
  ""Id"" varchar(20) COLLATE ""pg_catalog"".""default"" NOT NULL,
  ""Name"" varchar(255) COLLATE ""pg_catalog"".""default"" NOT NULL,
  ""SagaId"" varchar(20) COLLATE ""pg_catalog"".""default"" NOT NULL,
  ""Order"" int4 NOT NULL,
  ""Status"" varchar(10) COLLATE ""pg_catalog"".""default"" NOT NULL,
  ""OvertimeCompensateTimes"" int4 NOT NULL,
  ""ParameterData"" bytea NOT NULL,
  ""CompensateErrors"" varchar(1024) COLLATE ""pg_catalog"".""default"",
  ""CompensateTimes"" int4 NOT NULL,
  ""AddTime"" timestamp NOT NULL,
  CONSTRAINT ""Activities_pkey"" PRIMARY KEY (""Id""),
  CONSTRAINT ""Activities_SagaId_fkey"" FOREIGN KEY (""SagaId"") REFERENCES {GetSagaTableName()} (""Id"") ON DELETE CASCADE ON UPDATE NO ACTION
);

CREATE INDEX IF NOT EXISTS ""Activities_SagaId"" ON {GetActivityTableName()} USING btree (
  ""SagaId"" COLLATE ""pg_catalog"".""default"" ""pg_catalog"".""text_ops"" ASC NULLS LAST
);
            ";
            return batchSql;
        }
    }
}
