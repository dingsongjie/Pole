using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Domain.UnitOfWork
{
    public interface IUnitOfWorkManager
    {
        Task<IUnitOfWork> BeginUnitOfWork();
    }
}
