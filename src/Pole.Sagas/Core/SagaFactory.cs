using Microsoft.Extensions.Options;
using Pole.Core.Serialization;
using Pole.Core.Utils.Abstraction;
using Pole.Sagas.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    class SagaFactory : ISagaFactory
    {
        private readonly ISnowflakeIdGenerator snowflakeIdGenerator;
        private readonly IServiceProvider serviceProvider;
        private readonly IEventSender  eventSender;
        private readonly PoleSagasOption poleSagasOption;
        private readonly ISerializer serializer;
        public SagaFactory(ISnowflakeIdGenerator snowflakeIdGenerator, IServiceProvider serviceProvider, IEventSender eventSender, IOptions<PoleSagasOption> poleSagasOption, ISerializer serializer)
        {
            this.snowflakeIdGenerator = snowflakeIdGenerator;
            this.serviceProvider = serviceProvider;
            this.eventSender = eventSender;
            this.poleSagasOption = poleSagasOption.Value;
            this.serializer = serializer;
        }

        public ISaga CreateSaga()
        {
            return new Saga(snowflakeIdGenerator, serviceProvider, eventSender, poleSagasOption, serializer);
        }
    }
}
