using System;
using System.Threading.Tasks;

namespace Pole.Core.EventBus
{
    public interface IProducerContainer
    {
        ValueTask<IProducer> GetProducer<T>();
        ValueTask<IProducer> GetProducer(string typeName);
    }
}
