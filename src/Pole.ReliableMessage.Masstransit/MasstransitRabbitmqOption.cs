using GreenPipes;
using GreenPipes.Configurators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Masstransit
{
    public class MasstransitRabbitmqOption
    {
        public string RabbitMqHostAddress { get; set; }
        public string RabbitMqHostUserName { get; set; }
        public string RabbitMqHostPassword { get; set; }
        public string QueueNamePrefix { get; set; } = string.Empty;
        /// <summary>
        /// 2 个并发
        /// </summary>
        public ushort PrefetchCount { get; set; } = 2;

        public Action<IRetryConfigurator> RetryConfigure { get; set; } =
        r => r.Intervals(TimeSpan.FromSeconds(0.1)
                       , TimeSpan.FromSeconds(1)
                       , TimeSpan.FromSeconds(4)
                       , TimeSpan.FromSeconds(16)
                       , TimeSpan.FromSeconds(64)
                       );
    }
}
