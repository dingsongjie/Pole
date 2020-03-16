using System;
using System.Threading.Tasks;

namespace Pole.EventBus.Abstraction
{
    public interface IProducerInfoContainer
    {
        string GetTargetName(string typeName);
    }
}
