using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Messaging
{
    public class MessageCheckerResult
    {
        public static MessageCheckerResult NotFinished = new MessageCheckerResult(false);
        public MessageCheckerResult(bool isFinished,object @event):this(isFinished)
        {
            Event = @event;
        }
        public MessageCheckerResult(bool isFinished)
        {
            IsFinished = isFinished;
        }
        public bool IsFinished { get; set; }
        public object Event { get; set; }
    }
}
