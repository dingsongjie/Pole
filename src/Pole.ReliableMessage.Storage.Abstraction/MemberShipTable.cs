using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Storage.Abstraction
{
    public class MemberShipTable
    {
        public string Id { get;private set; }
        public MemberShipTable(string serviceName,string pendingMessageCheckerIp,DateTime iAmAliveUTCTime)
        {
            ServiceName = serviceName;
            PendingMessageCheckerIp = pendingMessageCheckerIp;
            IAmAliveUTCTime = iAmAliveUTCTime;
        }

        public string ServiceName { get;private set; }
        public string PendingMessageCheckerIp { get; private set; }
        public DateTime IAmAliveUTCTime { get; private set; }
    }
}
