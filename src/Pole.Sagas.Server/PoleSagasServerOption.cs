using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Server
{
    public class PoleSagasServerOption
    {
        /// <summary>
        /// 从数据库获取未结束的 sagas 的 时间间隔 单位秒
        /// </summary>
        public int NotEndedSagasFetchIntervalSeconds { get; set; } = 30;
        /// <summary>
        /// 每个Grpc 获取Sagas 的请求 ,服务端流式返回,每一次返回的间隔时间 单位秒
        /// </summary>
        public int GetSagasGrpcStreamingResponseDelaySeconds { get; set; } = 20;
        /// <summary>
        /// 过期数据 批量删除触发的时间间隔,单位秒
        /// </summary>
        public int ExpiredDataBulkDeleteIntervalSeconds { get; set; } = 10*60;
        /// <summary>
        /// 过期数据 批量是每一次删除的数量
        /// </summary>
        public int ExpiredDataDeleteBatchCount { get; set; } = 1000;
        /// <summary>
        /// 批量删除时 实际过期的数量比预定数量要大时,会分多次删除,此值为其中每次分批删除的时间间隔
        /// </summary>
        public int ExpiredDataPreBulkDeleteDelaySeconds { get; set; } = 3;
        public int PrometheusErrorSagasGaugeIntervalSeconds { get; set; } = 30;


    }
}
