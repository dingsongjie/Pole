using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Storage.PostgreSql
{
    public class PoleSagasStoragePostgreSqlOption
    {
        public string SagaTableName { get; set; }
        public string SchemaName { get; set; }
        public string ActivityTableName { get; set; }
        public int SagasRecoveryIntervalSecond { get; set; }
        public string ConnectionString { get; set; }
    }
}
