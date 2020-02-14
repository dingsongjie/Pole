using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core
{
    public class ProducerOptions
    {
        public int MaxFailedRetryCount { get; set; } = 40;
    }
}
