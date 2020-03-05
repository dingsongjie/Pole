using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core.Exceptions
{
   public class ActivityNotFoundByNameException:Exception
    {
        public ActivityNotFoundByNameException(string activityName) : base($"Activity not found by name: {activityName} ")
        {

        }
    }
}
