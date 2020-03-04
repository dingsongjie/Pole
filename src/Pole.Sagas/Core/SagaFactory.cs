using Pole.Core.Utils.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    class SagaFactory : ISagaFactory
    {
        private readonly ISnowflakeIdGenerator _snowflakeIdGenerator;
        public SagaFactory(ISnowflakeIdGenerator snowflakeIdGenerator)
        {
            _snowflakeIdGenerator = snowflakeIdGenerator;
        }

        public TSaga CreateSaga<TSaga>(TimeSpan timeOut) where TSaga : ISaga
        {
            var name = typeof(TSaga).FullName;
            var SagaFlow = SagasCollection.Get(name);
            var newId = _snowflakeIdGenerator.NextId();
            throw new NotImplementedException();
        }
    }
}
