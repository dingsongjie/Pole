using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backet.Api.Grains.Abstraction
{
    public interface IAddBacketGrain : IGrainWithStringKey
    {
        Task<bool> AddBacket(BacketDto backet);
    }
}
