using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core.Exceptions
{
    public class SagaNotFoundException:Exception
    {
        public SagaNotFoundException(string sagaName):base($"Saga:{sagaName} not found")
        {

        }
    }
}
