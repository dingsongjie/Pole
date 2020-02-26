using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pole.Core.EventBus
{
    public interface IProducer
    {
        ValueTask Publish(string targetName, byte[] bytes);
        ValueTask BulkPublish(IEnumerable<(string,byte[])> events);
    }
}
