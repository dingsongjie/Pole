using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Client.Abstraction
{
    public interface ISagaFactory
    {
        ISaga CreateSaga();
    }
}
