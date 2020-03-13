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
        public string ParameterData { get; set; }
        public int OvertimeCompensateTimes { get; set; }
        public int CompensateTimes  { get; set; }
    }
}
