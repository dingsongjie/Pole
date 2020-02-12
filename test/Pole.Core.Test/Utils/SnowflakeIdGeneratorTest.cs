using System;
using Xunit;

namespace Pole.Core.Test
{
    public class SnowflakeIdGeneratorTest
    {
        [Fact]
        public void MaxYears()
        {
            var years = -1L ^ (-1L << 6);
            Console.WriteLine(years);
        }
    }
}
