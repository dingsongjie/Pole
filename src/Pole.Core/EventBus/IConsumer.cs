using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pole.Core.EventBus
{
    public interface IConsumer
    {
        Task Notice(byte[] bytes);
        Task Notice(List<byte[]> list);
    }
}
