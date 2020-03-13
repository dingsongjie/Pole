using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public class ActivityEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SagaId { get; set; }
        public int Order { get; set; }
        public string Status { get; set; }
        public int TimeOutSeconds { get; set; }
        public Byte[] ParameterData { get; set; }
        public Byte[] ResultData { get; set; }
        public string Errors { get; set; }
        public int OvertimeCompensateTimes { get; set; }
        public int CompensateTimes  { get; set; }
        public DateTime AddTime { get; set; }
    }
}
