using System.Threading.Tasks;
using Orleans;

namespace Pole.Core.Services
{
    public interface ILock : IGrainWithStringKey
    {
        Task<bool> Lock(int millisecondsDelay = 0);
        Task Unlock();
    }
}
