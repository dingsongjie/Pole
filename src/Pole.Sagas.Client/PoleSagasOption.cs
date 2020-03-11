using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Client
{
    public class PoleSagasOption
    {
        public string ServiceName { get; set; }
        public int PreSagasGrpcStreamingResponseLimitCount { get; set; } = 20;
        public int MaxCompensateTimes { get; set; } = 10;
        public int MaxOvertimeCompensateTimes { get; set; } = 10;
        public int CompeletedSagaExpiredAfterSeconds { get; set; } = 60 * 10;
        public int SagasTimeOutSeconds { get; set; } = 60;
        public string SagasServerHost { get; set; }
    }
}
