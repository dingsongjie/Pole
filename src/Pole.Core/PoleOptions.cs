using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core
{
    public class PoleOptions
    {
        public int PendingMessageRetryIntervalSeconds { get; set; } = 30;
        public IServiceCollection Services { get; private set; }
    }
}
