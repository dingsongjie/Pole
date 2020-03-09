using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public enum ActivityRetryType
    {
        Execute,
        Compensate
    }
}
