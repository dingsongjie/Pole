using System.Threading.Tasks;

namespace Pole.Core.EventBus
{
    public interface IProducer
    {
        ValueTask Publish(byte[] bytes, string hashKey);
    }
}
