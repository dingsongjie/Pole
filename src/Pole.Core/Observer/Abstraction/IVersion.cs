using System.Threading.Tasks;

namespace Pole.Core.Observer
{
    public interface IVersion
    {
        Task<long> GetVersion();
        Task<long> GetAndSaveVersion(long compareVersion);
    }
}
