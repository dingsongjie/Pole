using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pole.EventBus
{
    public interface IConsumer
    {
        Task Notice(byte[] bytes);
        Task Notice(List<byte[]> list);
    }
}
