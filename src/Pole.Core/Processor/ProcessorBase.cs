using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Core.Processor
{
    public abstract class ProcessorBase : IProcessor
    {
        public abstract string Name { get; }

        public abstract Task Process(ProcessingContext context);

        public override string ToString()
        {
            return Name;
        }
    }
}
