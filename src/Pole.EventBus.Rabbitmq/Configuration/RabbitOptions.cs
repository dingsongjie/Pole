using System.Collections.Generic;
using RabbitMQ.Client;

namespace Pole.EventBus.RabbitMQ
{
    public class RabbitOptions
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public int MasChannelsPerConnection { get; set; } = 200;
        /// <summary>
        /// 目前为一个连接 当消息数量非常大时,单个TCP连接的运输能力有限,可以修改这个最大连接数提高运输能力
        /// </summary>
        public int MaxConnection { get; set; } = 1;
        /// <summary>
        /// 消费者批量处理每次处理的最大消息量
        /// </summary>
        public ushort CunsumerMaxBatchSize { get; set; } = 3000;
        /// <summary>
        /// 消费者批量处理每次处理的最大延时
        /// </summary>
        public int CunsumerMaxMillisecondsInterval { get; set; } = 1000;
        /// <summary>
        /// exchange 和 queue 名称的前缀
        /// </summary>
        public string Prefix = "Pole_";
        public string[] Hosts
        {
            get; set;
        }
        public List<AmqpTcpEndpoint> EndPoints
        {
            get
            {
                var list = new List<AmqpTcpEndpoint>();
                foreach (var host in Hosts)
                {
                    list.Add(AmqpTcpEndpoint.Parse(host));
                }
                return list;
            }
        }
    }
}
