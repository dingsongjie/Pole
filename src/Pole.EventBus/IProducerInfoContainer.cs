using System;
using System.Threading.Tasks;

namespace Pole.EventBus
{
    public interface IProducerInfoContainer
    {
        string GetTargetName(string typeName);
    }
}
