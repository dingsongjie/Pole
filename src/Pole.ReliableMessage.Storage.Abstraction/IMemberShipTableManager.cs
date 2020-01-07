using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Storage.Abstraction
{
    public interface IMemberShipTableManager
    {
        Task<bool> IsPendingMessageCheckerServiceInstance(string ipAddress);
        Task<bool> UpdateIAmAlive(string ipAddress, DateTime dateTime);
        /// <summary>
        /// 如果当前 超时时间内 没有可用 实例 返回 空
        /// </summary>
        /// <param name="iamAliveTimeout"></param>
        /// <returns></returns>
        Task<string> GetPendingMessageCheckerServiceInstanceIp(DateTime iamAliveEndTime);

        Task<bool> AddCheckerServiceInstanceAndDeleteOthers(string ipAddress, DateTime aliveUTCTime);
    }
}
