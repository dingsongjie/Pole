using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public enum ActivityStatus
    {
        NotStarted,
        Executing,
        Executed,
        Compensating,
        Compensated,
        ExecuteAborted,
        Revoked,
        CompensateAborted,
        ExecutingOvertime
    }
}
