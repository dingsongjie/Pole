using Pole.ReliableMessage.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.ReliableMessage.Utils
{
    class DefaulTimeHelper : ITimeHelper
    {
        public string GetAppropriateFormatedDateString()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public DateTime GetUTCNow()
        {
            return DateTime.UtcNow;
        }
    }
}
