using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Abstraction
{
    public interface ITimeHelper
    {
        DateTime GetUTCNow();
        /// <summary>
        /// "UTC :{_timeHelper.GetNow().ToString("yyyy-MM-dd HH:mm:ss.fff")}"
        /// </summary>
        /// <returns></returns>
        string GetAppropriateFormatedDateString();
    }
}
