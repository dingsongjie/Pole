using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Storage.Mongodb
{
    public class MongodbOption
    {
        public string MessageDatabaseName { get; set; } = "ReliableMessage";
        public string MembershipCollectionName { get; set; } = "Membership";
        /// <summary>
        ///  bucket 中最大消息数 一旦达到最大数量 后面的数据将覆盖前面的数据
        /// </summary>
        public long CollectionMaxMessageCount { get; set; } = 20000000;

        /// <summary>
        /// 默认最大为10G
        /// </summary>
        public long CollectionMaxSize { get; set; } = 10*1024*1024*1024L;

        public string ServiceCollectionName { get; set; }
        public MongoHost[] Servers { get; set; }
    }

}
