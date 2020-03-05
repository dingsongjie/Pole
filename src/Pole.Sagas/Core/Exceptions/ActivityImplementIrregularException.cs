using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core.Exceptions
{
    class ActivityImplementIrregularException: Exception
    {
        public ActivityImplementIrregularException(string name) : base($"Activity name :{name }must have and only inherit from IActivity<>")
        {

        }
    }
}
