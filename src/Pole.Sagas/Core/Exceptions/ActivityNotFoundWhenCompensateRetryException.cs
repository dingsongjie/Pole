using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core.Exceptions
{
    public class ActivityNotFoundWhenCompensateRetryException : Exception
    {
        public ActivityNotFoundWhenCompensateRetryException(string activityName):base($"Activity:{activityName} NotFound When Compensate Retry")
        {

        }
    }
}
