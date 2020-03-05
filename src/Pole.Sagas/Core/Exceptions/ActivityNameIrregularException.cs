using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core.Exceptions
{
   public class ActivityNameIrregularException:Exception
    {
        public ActivityNameIrregularException(Type activityType):base($"Activity : {activityType.FullName} irregular naming")
        {

        }
    }
}
