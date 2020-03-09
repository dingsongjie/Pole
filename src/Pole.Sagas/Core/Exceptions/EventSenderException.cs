using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core.Exceptions
{
    public class SagasServerException : Exception
    {
        public SagasServerException(string errors) : base(errors)
        {

        }
    }
}
