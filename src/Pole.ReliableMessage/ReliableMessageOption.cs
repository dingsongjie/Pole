using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pole.ReliableMessage
{
    public class ReliableMessageOption
    {
        public List<IReliableMessageOptionExtension> ReliableMessageOptionExtensions { get; private set; } = new List<IReliableMessageOptionExtension>();
        public List<Assembly> EventCallbackAssemblies { get; private set; }
        public List<Assembly> EventHandlerAssemblies { get; private set; }
        /// <summary>
        /// 待发送消息重试间隔 单位 秒
        /// </summary>
        public int PendingMessageRetryInterval { get; set; } = 10;

        /// <summary>
        ///  待发送消息最大重试次数  , 0 为 无上限
        /// </summary>
        public int PendingMessageRetryTimes { get; set; } = 0;

        /// <summary>
        /// 预发送消息超时时间 单位 秒
        /// </summary>
        public int PendingMessageTimeOut { get; set; } = 10 * 60;

        /// <summary>
        /// 预发送消息检查时每一次获取的消息数量 
        /// </summary>
        public int PendingMessageCheckBatchCount { get; set; } = 1000;

        /// <summary>
        /// 预发送消息状态检查最后时间 单位 秒
        /// </summary>
        public int PendingMessageCheckingTimeOutSeconds { get; set; } = 13 * 60;

        /// <summary>
        /// 已发送的消息缓冲区 flush to storage 的时间间隔 单位 秒
        /// </summary>
        public int PushedMessageFlushInterval { get; set; } = 2;


        /// <summary>
        /// PendingMessage 第一次处理等待时间 单位 秒
        /// </summary>
        public int PendingMessageFirstProcessingWaitTime { get; set; } = 2 + 10;

        /// <summary>
        /// 每次重试之间最大间隔 单位 秒
        /// </summary>
        public int MaxPendingMessageRetryDelay { get; set; } = 2 * 60;

        /// <summary>
        /// PendingMessageCheck 实例更新 存活时间的时间间隔 单位 秒
        /// </summary>
        public int PendingMessageCheckerInstanceIAnAliveTimeUpdateDelay { get; set; } = 10;

        /// <summary>
        /// PendingMessageCheck 实例存活超时时间 单位 秒
        /// </summary>
        public int PendingMessageCheckerInstanceIAnAliveTimeout { get; set; } = 3 * 10;

        /// <summary>
        /// Message 定期清理时间间隔 单位 秒
        /// </summary>
        public int MessageCleanInterval { get; set; } = 30 * 60;

        /// <summary>
        /// 当主机有多个网络时通过指定网关地址找到合适的服务ip地址
        /// </summary>
        public string NetworkInterfaceGatewayAddress { get; set; } = string.Empty;

        public ReliableMessageOption AddEventAssemblies(params Assembly[] assemblies)
        {
            EventCallbackAssemblies = assemblies.ToList();
            return this;
        }
        public ReliableMessageOption AddEventAssemblies(IEnumerable<Assembly> assemblies)
        {
            EventCallbackAssemblies = assemblies.ToList();
            return this;
        }
        public ReliableMessageOption AddEventHandlerAssemblies(params Assembly[] assemblies)
        {
            EventHandlerAssemblies = assemblies.ToList();
            return this;
        }
        public ReliableMessageOption AddEventHandlerAssemblies(IEnumerable<Assembly> assemblies)
        {
            EventHandlerAssemblies = assemblies.ToList();
            return this;
        }
    }
}
