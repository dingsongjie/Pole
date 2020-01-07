using GreenPipes.Configurators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Masstransit
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ReliableEventHandlerParemeterAttribute : Attribute
    {
        public ushort PrefetchCount { get; set; }
        public QueueHaTypeEnum QueueHaType { get; set; } = QueueHaTypeEnum.Default;
        
    }
}
