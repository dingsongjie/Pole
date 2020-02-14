using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Core.Processor
{
    public class LoopProcessor : ProcessorBase
    {
        private IProcessor _processor;
        private readonly ILoggerFactory _loggerFactory;

        public LoopProcessor(IProcessor processor, ILoggerFactory loggerFactory)
        {
            _processor = processor;
            _loggerFactory = loggerFactory;
        }
        public override string Name => "LoopProcessor";
        public override async Task Process(ProcessingContext context)
        {
            var logger = _loggerFactory.CreateLogger<LoopProcessor>();

            while (!context.IsStopping)
            {
                try
                {
                    logger.LogDebug($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")}...{ this.ToString() } process start");

                    await _processor.Process(context);

                    logger.LogDebug($"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")}...{ this.ToString() } process compelete");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")}...{ this.ToString() } process error");
                }
            }
        }
        public override string ToString()
        {
            var strArray = new string[2];
            strArray[0] = Name;
            strArray[1] = _processor.Name;
            return string.Join("_", strArray);
        }
    }
}
