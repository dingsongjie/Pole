using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core.Exceptions
{
    public class ActivityNameRepeatedException : Exception
    {
        public ActivityNameRepeatedException(string name):base($"Activity :{name}  already exists")
        {

        }
    }
}
