using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Domain.UnitOfWork
{
    public enum WorkerStatus
    {
        Init = 0,
        PreCommited = 1,
        Commited = 2,
        PostCommited = 3,
        Rollbacked = 4
    }
}
