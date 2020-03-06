using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    enum SagaStatus
    {
        Started,
        Ended,
        Error,
        Overtime
    }
}
