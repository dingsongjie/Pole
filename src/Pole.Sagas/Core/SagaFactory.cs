using Microsoft.Extensions.Options;
using Pole.Core.Serialization;
using Pole.Core.Utils.Abstraction;
using Pole.Sagas.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public class SagaFactory : ISagaFactory
    {
        private readonly ISnowflakeIdGenerator snowflakeIdGenerator;
        private readonly IServiceProvider serviceProvider;
        private readonly IEventSender eventSender;
        private readonly PoleSagasOption poleSagasOption;
        private readonly ISerializer serializer;
        private readonly IActivityFinder activityFinder;
        public SagaFactory(ISnowflakeIdGenerator snowflakeIdGenerator, IServiceProvider serviceProvider, IEventSender eventSender, IOptions<PoleSagasOption> poleSagasOption, ISerializer serializer, IActivityFinder activityFinder)
        {
            this.snowflakeIdGenerator = snowflakeIdGenerator;
            this.serviceProvider = serviceProvider;
            this.eventSender = eventSender;
            this.poleSagasOption = poleSagasOption.Value;
            this.serializer = serializer;
            this.activityFinder = activityFinder;
        }

        public ISaga CreateSaga()
        {
            return new Saga(snowflakeIdGenerator, serviceProvider, eventSender, poleSagasOption, serializer, activityFinder);
        }
        internal ISaga CreateSaga(string id)
        {
            return new Saga(snowflakeIdGenerator, serviceProvider, eventSender, poleSagasOption, serializer, activityFinder);
        }
    }
}
