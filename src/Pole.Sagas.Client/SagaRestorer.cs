using Microsoft.Extensions.Options;
using Pole.Core.Serialization;
using Pole.Core.Utils.Abstraction;
using Pole.Sagas.Client.Abstraction;
using Pole.Sagas.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pole.Sagas.Client
{
    class SagaRestorer
    {
        private readonly ISnowflakeIdGenerator snowflakeIdGenerator;
        private readonly IServiceProvider serviceProvider;
        private readonly IEventSender eventSender;
        private readonly PoleSagasOption poleSagasOption;
        private readonly ISerializer serializer;
        private readonly IActivityFinder activityFinder;
        public SagaRestorer(ISnowflakeIdGenerator snowflakeIdGenerator, IServiceProvider serviceProvider, IEventSender eventSender, PoleSagasOption poleSagasOption, ISerializer serializer, IActivityFinder activityFinder)
        {
            this.snowflakeIdGenerator = snowflakeIdGenerator;
            this.serviceProvider = serviceProvider;
            this.eventSender = eventSender;
            this.poleSagasOption = poleSagasOption;
            this.serializer = serializer;
            this.activityFinder = activityFinder;
        }
        internal Saga CreateSaga(SagaEntity sagaEntity)
        {
            var saga = new Saga(snowflakeIdGenerator, serviceProvider, eventSender, poleSagasOption, serializer, activityFinder, sagaEntity.Id);
            foreach (var activity in sagaEntity.ActivityEntities.OrderBy(m=>m.Order))
            {
                saga.AddActivity(activity.Id,activity.Name, activity.Status, activity.ParameterData, activity.Order, activity.CompensateTimes,activity.OvertimeCompensateTimes);
            }
            return saga;
        }
    }
}
