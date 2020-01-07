using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Messaging
{
    public class Message : IComparable<Message>
    {
        /// <summary>
        /// 这里id 永远为 string 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 消息状态
        /// </summary>
        public MessageStatus MessageStatus { get; set; }

        /// <summary>
        /// 消息状态Id
        /// </summary>

        public int MessageStatusId { get; set; }

        /// <summary>
        /// 预发送的时间
        /// </summary>
        public DateTime AddedUTCTime { get; set; }

        /// <summary>
        /// 用来存放 消息内容  目前没有大小限制 这个需要根据 实际情况 , mongodb 和 rabiitmq 的 综合指标来定 ,开发人员 在使用超大对象时需谨慎
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 消息的名称 用来鉴别不同的消息
        /// </summary>
        public string MessageTypeId { get; set; }

        /// <summary>
        /// 当前消息 回调者所需参数值
        /// </summary>
        public string RePushCallBackParameterValue { get; set; }

        ///// <summary>
        ///// 最后一次的重试时间
        ///// </summary>
        //public DateTime LastRetryUTCTime { get; set; }


        /// <summary>
        /// 下一次的重试时间
        /// </summary>
        public DateTime NextRetryUTCTime { get; set; }

        /// <summary>
        /// 重试次数 
        /// </summary>
        public int RetryTimes { get; set; } = 0;

        public int CompareTo(Message other)
        {
            return Id.CompareTo(other.Id);
        }
    }
    public class MessageIEqualityComparer : IEqualityComparer<Message>
    {
        public static MessageIEqualityComparer Default = new MessageIEqualityComparer();
        public bool Equals(Message x, Message y)
        {
            return x.CompareTo(y) == 0;
        }

        public int GetHashCode(Message obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
