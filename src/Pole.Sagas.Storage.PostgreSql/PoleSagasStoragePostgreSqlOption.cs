using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Storage.PostgreSql
{
    public class PoleSagasStoragePostgreSqlOption
    {
        public string SagaTableName { get; set; } = "Sagas";
        public string SchemaName { get; set; } = "pole-sagas";
        public string ActivityTableName { get; set; } = "Activities";
        public string OvertimeCompensationGuaranteeTableName { get; set; } = "OCG-Activities";
        public int SagasRecoveryIntervalSecond { get; set; }
        public string ConnectionString { get; set; }
    }
}
