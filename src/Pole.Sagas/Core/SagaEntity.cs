using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public class SagaEntity
    {
        public string Id { get; set; }
        public string ServiceName { get; set; }
        public List<ActivityEntity> ActivityEntities { get; set; }
        public string Status { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime AddTime { get; set; }
    }
}
