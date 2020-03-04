using Pole.Sagas.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public class SagasCollection : Dictionary<string, ISaga>
    {
        private static System.Collections.Concurrent.ConcurrentDictionary<string, ISaga> _sagas = new System.Collections.Concurrent.ConcurrentDictionary<string, ISaga>();
        public static ISaga Get(string name)
        {
            if (!_sagas.TryGetValue(name, out ISaga saga))
            {
                throw new SagaNotFoundException(name);
            }
            return saga;
        }
        public static bool Add(ISaga saga)
        {
            var name = saga.GetType().FullName;
            return _sagas.TryAdd(name, saga);
        }
    }
}
