using Pole.ReliableMessage.Processor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Abstraction
{
    public interface IProcessor
    {
        string Name { get; }
        Task Process(ProcessingContext context);
    }
}
