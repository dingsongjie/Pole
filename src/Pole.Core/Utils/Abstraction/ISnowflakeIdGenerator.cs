using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core.Utils.Abstraction
{
    public interface ISnowflakeIdGenerator
    {
        public string NextId();
    }
}
