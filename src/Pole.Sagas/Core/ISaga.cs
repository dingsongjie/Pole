using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public interface ISaga
    {
        void AddActivity<TData>(IActivity<TData> activity);
    }
}
