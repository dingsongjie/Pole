using Pole.ReliableMessage.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Processor
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
