using Pole.Core.Serialization;
using Pole.Core.Utils.Abstraction;
using Pole.Sagas.Core.Abstraction;
using Pole.Sagas.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Sagas.Core
{
    public class Saga : ISaga
    {
        private List<ActivityWapper> activities = new List<ActivityWapper>();
        private IServiceProvider serviceProvider;
        private IEventSender eventSender;
        private ISnowflakeIdGenerator snowflakeIdGenerator;
        private IActivityFinder activityFinder;
        private PoleSagasOption poleSagasOption;
        private int _currentMaxOrder = 0;
        private int _currentExecuteOrder = 0;
        private int _currentCompensateOrder = 0;
        private ISerializer serializer;
        public string Id { get; }

        public Saga(ISnowflakeIdGenerator snowflakeIdGenerator, IServiceProvider serviceProvider, IEventSender eventSender, PoleSagasOption poleSagasOption, ISerializer serializer, IActivityFinder activityFinder)
        {
            this.snowflakeIdGenerator = snowflakeIdGenerator;
            this.serviceProvider = serviceProvider;
            this.eventSender = eventSender;
            this.poleSagasOption = poleSagasOption;
            this.serializer = serializer;
            this.activityFinder = activityFinder;
            Id = snowflakeIdGenerator.NextId();
        }

        public void AddActivity(string activityName, object data)
        {
            var targetActivityType = activityFinder.FindType(activityName);

            var activityInterface = targetActivityType.GetInterfaces().FirstOrDefault();
            if (!activityInterface.IsGenericType)
            {
                throw new ActivityImplementIrregularException(activityName);
            }
            var dataType = activityInterface.GetGenericArguments()[0];
            _currentMaxOrder++;
            ActivityWapper activityWapper = new ActivityWapper
            {
                ActivityDataType = dataType,
                ActivityState = ActivityStatus.NotStarted,
                ActivityType = targetActivityType,
                DataObj = data,
                Order = _currentMaxOrder,
                ServiceProvider = serviceProvider
            };
            activities.Add(activityWapper);
        }

        public async Task<SagaResult> GetResult()
        {
            await eventSender.SagaStarted(Id, poleSagasOption.ServiceName);

            var executeActivity = GetNextExecuteActivity();
            if (executeActivity == null)
            {
                var expiresAt = DateTime.UtcNow.AddSeconds(poleSagasOption.CompeletedSagaExpiredAfterSeconds);
                await eventSender.SagaEnded(Id, expiresAt);
                return SagaResult.SuccessResult;
            }
            var result = await RecursiveExecuteActivity(executeActivity);
            return result;
        }

        private ActivityWapper GetNextExecuteActivity()
        {
            if (_currentExecuteOrder == _currentMaxOrder)
            {
                return null;
            }
            _currentExecuteOrder++;
            return activities[_currentExecuteOrder-1];
        }
        private ActivityWapper GetNextCompensateActivity()
        {
            _currentCompensateOrder--;
            if (_currentCompensateOrder == 0)
            {
                return null;
            }

            return activities[_currentCompensateOrder-1];
        }
        private async Task RecursiveCompensateActivity(ActivityWapper activityWapper)
        {
            var activityId = activityWapper.Id;
            try
            {
                await activityWapper.InvokeCompensate();
                await eventSender.ActivityCompensated(activityId);
                var compensateActivity = GetNextCompensateActivity();
                if (compensateActivity == null)
                {
                    return;
                }
                await RecursiveCompensateActivity(compensateActivity);
            }
            catch (Exception exception)
            {
                await eventSender.ActivityCompensateAborted(activityId, Id, exception.InnerException != null ? exception.InnerException.Message + exception.StackTrace : exception.Message + exception.StackTrace);
            }
        }
        private async Task<ActivityExecuteResult> RecursiveExecuteActivity(ActivityWapper activityWapper)
        {
            var activityId = snowflakeIdGenerator.NextId();
            activityWapper.Id = activityId;
            try
            {
                var jsonContent = serializer.Serialize(activityWapper.DataObj, activityWapper.ActivityDataType);
                await eventSender.ActivityExecuteStarted(activityId, Id, activityWapper.TimeOut, jsonContent, activityWapper.Order);
                var result = await activityWapper.InvokeExecute();
                if (!result.IsSuccess)
                {
                    await eventSender.ActivityExecuteAborted(activityId, serializer.Serialize(result.Result), string.Empty);
                    _currentCompensateOrder = _currentExecuteOrder;
                    var compensateActivity = GetNextCompensateActivity();
                    await RecursiveCompensateActivity(compensateActivity);
                    return result;
                }
                await eventSender.ActivityEnded(activityId, string.Empty);
                var executeActivity = GetNextExecuteActivity();
                if (executeActivity == null)
                {
                    return result;
                }
                else
                {
                    return await RecursiveExecuteActivity(executeActivity);
                }
            }
            catch (Exception exception)
            {
                var errors = exception.InnerException != null ? exception.InnerException.Message + exception.StackTrace : exception.Message + exception.StackTrace;
                await eventSender.ActivityExecuteAborted(activityId, string.Empty, errors);
                return new ActivityExecuteResult
                {
                    IsSuccess = false,
                    Errors = errors
                };
            }
        }
    }
}
