using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.EventBus.EventStorage
{
    public interface IEventStorageInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken);
        string GetTableName();
    }
}
