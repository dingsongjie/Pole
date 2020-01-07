using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Storage.Mongodb
{
    public sealed class MongoHost
    {
        /// <summary>
        /// 主机或者IP地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }
    }
}
