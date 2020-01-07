using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Pole.ReliableMessage.Processor
{
    public class ProcessingContext
    {
        public ProcessingContext(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }
        public CancellationToken CancellationToken { get; }
        public bool IsStopping => CancellationToken.IsCancellationRequested;
    }
}
