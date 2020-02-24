using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core.Utils.Abstraction
{
    public  class SnowflakeIdGenerator : ISnowflakeIdGenerator
    {

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private int generatorIdBits;
        private long twepoch;
        private int maxGeneratorId;
        private const int SequenceAndGeneratorIdBits = 64 - 1 - 41;
        /// <summary>
        /// 这里的位数决定 每毫秒能生成的最大个数 
        /// </summary>
        private int sequenceBits;
        private int generatorIdShift;
        private const int TimestampLeftShift = SequenceAndGeneratorIdBits;
        private long sequenceMask;
        public long GeneratorId { get; private set; }

        // 毫秒内序列(0~4095) 
        public long Sequence { get; private set; }

        // 上次生成ID的时间截 
        public long LastTimestamp { get; private set; }

        /// <summary>
        /// 时间戳为41位,可以使用69年,年T = (1L << 41) / (1000L * 60 * 60 * 24 * 365) = 69
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="generatorIdBits"></param>
        public SnowflakeIdGenerator(DateTime beginTime, int generatorIdBits, long generatorId)
        {
            twepoch = Convert.ToInt64((beginTime.ToUniversalTime() - Jan1st1970).TotalMilliseconds);
            this.generatorIdBits = generatorIdBits;
            maxGeneratorId = -1 ^ (-1 << this.generatorIdBits);
            sequenceBits = SequenceAndGeneratorIdBits - generatorIdBits;
            generatorIdShift = sequenceBits;
            sequenceMask = -1L ^ (-1L << sequenceBits);
            GeneratorId = generatorId;
            Sequence = 0L;
            LastTimestamp = -1L;
        }
        public string NextId()
        {
            lock (this)
            {
                long timestamp = GetCurrentTimestamp();
                if (timestamp > LastTimestamp) //时间戳改变，毫秒内序列重置
                {
                    Sequence = 0L;
                }
                else if (timestamp == LastTimestamp) //如果是同一时间生成的，则进行毫秒内序列
                {
                    Sequence = (Sequence + 1) & sequenceMask;
                    if (Sequence == 0) //毫秒内序列溢出
                    {
                        timestamp = GetNextTimestamp(LastTimestamp); //阻塞到下一个毫秒,获得新的时间戳
                    }
                }
                else   //当前时间小于上一次ID生成的时间戳，证明系统时钟被回拨，此时需要做回拨处理
                {
                    Sequence = (Sequence + 1) & sequenceMask;
                    if (Sequence > 0)
                    {
                        timestamp = LastTimestamp;     //停留在最后一次时间戳上，等待系统时间追上后即完全度过了时钟回拨问题。
                    }
                    else   //毫秒内序列溢出
                    {
                        timestamp = LastTimestamp + 1;   //直接进位到下一个毫秒                          
                    }
                }

                LastTimestamp = timestamp;       //上次生成ID的时间截

                //移位并通过或运算拼到一起组成64位的ID
                var id = ((timestamp - twepoch) << TimestampLeftShift)
                        | (GeneratorId << generatorIdShift)
                        | Sequence;
                return id.ToString();
            }
        }
        private static long GetCurrentTimestamp()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }
        private static long GetNextTimestamp(long lastTimestamp)
        {
            long timestamp = GetCurrentTimestamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetCurrentTimestamp();
            }
            return timestamp;
        }
    }
}
