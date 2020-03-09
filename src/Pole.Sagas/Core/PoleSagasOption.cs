using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public class PoleSagasOption
    {
        public string ServiceName { get; set; }
        public int CompeletedSagaExpiredAfterSeconds { get; set; } = 60 * 10;
        public string SagasServerHost { get; set; }
    }
}
