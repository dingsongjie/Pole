using GreenPipes;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.ReliableMessage.Masstransit.Pipe
{
    class AddReliableMessageIdPipe : IPipe<PublishContext>
    {
        public const string RELIABLE_MESSAGE_ID = "ReliableMessageId";
        private readonly string _reliableMessageId;
        public AddReliableMessageIdPipe(string reliableMessageId)
        {
            _reliableMessageId = reliableMessageId;
        }
        public void Probe(ProbeContext context)
        {
            
        }

        public async Task Send(PublishContext context)
        {
            context.Headers.Set(RELIABLE_MESSAGE_ID, _reliableMessageId);
            await Task.CompletedTask;
        }
    }
}
