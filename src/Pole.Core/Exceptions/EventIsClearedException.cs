using System;

namespace Pole.Core.Exceptions
{
    public class EventIsClearedException : Exception
    {
        public EventIsClearedException(string eventType, string eventJsonString, long archiveIndex) : base($"eventType:{eventType},event:{eventJsonString},archive index:{archiveIndex}")
        {
        }
    }
}
