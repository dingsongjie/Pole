using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core.Exceptions
{
    public class CompensateActivityException : Exception
    {
        public CompensateActivityException(string activityName,Exception innerException):base($"activity: {activityName} compensate error", innerException)
        {

        }
    }
}
