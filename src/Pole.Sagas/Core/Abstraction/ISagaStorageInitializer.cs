using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Sagas.Core.Abstraction
{
   public interface ISagaStorageInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken);
        string GetSagaTableName();
        string GetActivityTableName();
        string GetOvertimeCompensationGuaranteeTableName();
    }
}
