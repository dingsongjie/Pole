using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Core.Processor
{
    public interface IProcessor
    {
        string Name { get; }
        Task Process(ProcessingContext context);
    }
}
