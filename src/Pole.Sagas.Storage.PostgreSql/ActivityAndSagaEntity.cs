using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Storage.PostgreSql
{
    public class ActivityAndSagaEntity
    {
        public string Id { get; set; }
        public string SagaId { get; set; }
        public string ServiceName { get; set; }
        public int Order { get; set; }
        public string Status { get; set; }
        public byte[] ParameterData { get; set; }
        public int OvertimeCompensateTimes { get; set; }
        public int CompensateTimes { get; set; }
        public int Name { get; set; }
    }
}
