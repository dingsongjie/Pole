using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core.Exceptions
{
    public class ActivityNotFoundByTypeException : Exception
    {
        public ActivityNotFoundByTypeException(Type activityType) : base($"Activity not found by type: {activityType.FullName}")
        {

        }
    }
}
