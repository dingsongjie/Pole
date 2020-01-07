using Pole.ReliableMessage.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Masstransit
{
    public class QueueHaType : Enumeration
    {
        public static QueueHaType None = new QueueHaType(1, "无",string.Empty);
        public static QueueHaType Default = new QueueHaType(2, "默认高可用","Rmd.");
        public static QueueHaType Backlog = new QueueHaType(3, "消息可积压","Rmb.");
        private readonly string _queuePrefix;
        public QueueHaType(int id, string name) : base(id, name)
        {
        }
        public QueueHaType(int id, string name,string prefix) : this(id, name)
        {
            _queuePrefix = prefix;
        }
        public string GenerateQueueName(string rawQueueName)
        {
            return string.Concat(_queuePrefix, rawQueueName);
        }
    }
    public enum QueueHaTypeEnum:int
    {
        None = 1,
        Default = 2,
        Backlog = 3
    }
}
