using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Storage.Mongodb
{
    public class MongodbOption
    {
        public string MessageDatabaseName { get; set; } = "ReliableMessage";
        public string MembershipCollectionName { get; set; } = "Membership";
        public string ServiceCollectionName { get; set; }
        public MongoHost[] Servers { get; set; }
    }

}
