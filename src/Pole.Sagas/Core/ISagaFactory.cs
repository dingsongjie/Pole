using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public interface ISagaFactory
    {
        TSaga CreateSaga<TSaga>(TimeSpan timeOut) where TSaga : ISaga;
    }
}
