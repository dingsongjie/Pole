namespace Pole.EventBus.RabbitMQ
{
    /// <summary>
    /// Consumer配置信息
    /// </summary>
    public class ConsumerOptions
    {
        /// <summary>
        /// 消息处理失败是否重回队列
        /// </summary>
        public bool Reenqueue { get; set; }
        /// <summary>
        /// 错误队列后缀
        /// </summary>
        public string ErrorQueueSuffix { get; set; }
        /// <summary>
        /// 消息处理失败最大重试次数
        /// </summary>
        public int MaxReenqueueTimes { get; set; }
    }
}
