using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Core
{
    public static class Consts
    {
        public static ValueTask ValueTaskDone = new ValueTask();
        public const string ConsumerRetryTimesStr = "pole-consumer-retry-times";
        public const string ConsumerExceptionDetailsStr = "pole-consumer-exception-details";
        public const string EventHandlerMethodName = "EventHandle";
        public const string BatchEventsHandlerMethodName = "BatchEventsHandler";
    }
}
