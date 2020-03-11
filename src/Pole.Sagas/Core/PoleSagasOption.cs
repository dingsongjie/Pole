using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public class PoleSagasOption
    {
        public string ServiceName { get; set; }
        public int CompeletedSagaExpiredAfterSeconds { get; set; } = 60 * 10;
        public int SagasTimeOutSeconds { get; set; } = 60;
        public string SagasServerHost { get; set; }
    }
}
