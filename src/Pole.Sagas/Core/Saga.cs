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
        private int CurrentMaxOrder
        {
            get { return activities.Count; }
        }
        /// <summary>
        /// 如果 等于 -1 说明已经在执行补偿操作,此时这个值已经没有意义
        /// </summary>
        private int currentExecuteOrder  = 0;
        /// <summary>
        /// 如果 等于 -1 说明已经还未执行补偿操作,此时这个值没有意义
        /// </summary>
        private int currentCompensateOrder = -1;
        private ISerializer serializer;
        public string Id { get; }

        internal Saga(ISnowflakeIdGenerator snowflakeIdGenerator, IServiceProvider serviceProvider, IEventSender eventSender, PoleSagasOption poleSagasOption, ISerializer serializer, IActivityFinder activityFinder)
        {
            this.snowflakeIdGenerator = snowflakeIdGenerator;
            this.serviceProvider = serviceProvider;
            this.eventSender = eventSender;
            this.poleSagasOption = poleSagasOption;
            this.serializer = serializer;
            this.activityFinder = activityFinder;
            Id = snowflakeIdGenerator.NextId();
        }
        internal Saga(ISnowflakeIdGenerator snowflakeIdGenerator, IServiceProvider serviceProvider, IEventSender eventSender, PoleSagasOption poleSagasOption, ISerializer serializer, IActivityFinder activityFinder,int currentExecuteOrder,int currentCompensateOrder, List<ActivityWapper> activities)
        {
            this.snowflakeIdGenerator = snowflakeIdGenerator;
            this.serviceProvider = serviceProvider;
            this.eventSender = eventSender;
            this.poleSagasOption = poleSagasOption;
            this.serializer = serializer;
            this.activityFinder = activityFinder;
            Id = snowflakeIdGenerator.NextId();
            this.currentExecuteOrder = currentExecuteOrder;
            this.currentCompensateOrder = currentCompensateOrder;
            this.activities = activities;
        }

        public void AddActivity(string activityName, object data, int timeOutSeconds = 2)
        {
            var targetActivityType = activityFinder.FindType(activityName);

            var activityInterface = targetActivityType.GetInterfaces().FirstOrDefault();
            if (!activityInterface.IsGenericType)
            {
                throw new ActivityImplementIrregularException(activityName);
            }
            var dataType = activityInterface.GetGenericArguments()[0];
            ActivityWapper activityWapper = new ActivityWapper
            {
                ActivityDataType = dataType,
                ActivityStatus = ActivityStatus.NotStarted,
                ActivityType = targetActivityType,
                DataObj = data,
                Order = CurrentMaxOrder,
                ServiceProvider = serviceProvider,
                TimeOutSeconds = 2,
            };
            activities.Add(activityWapper);
        }

        public async Task<SagaResult> GetResult()
        {
            await eventSender.SagaStarted(Id, poleSagasOption.ServiceName,DateTime.UtcNow);

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
            if (currentExecuteOrder == CurrentMaxOrder)
            {
                return null;
            }
            currentExecuteOrder++;
            return activities[currentExecuteOrder - 1];
        }
        private ActivityWapper GetNextCompensateActivity()
        {
            currentCompensateOrder--;
            if (currentCompensateOrder == 0)
            {
                return null;
            }

            return activities[currentCompensateOrder - 1];
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
            activityWapper.CancellationTokenSource = new System.Threading.CancellationTokenSource(2 * 1000);
            try
            {
                var bytesContent = serializer.SerializeToUtf8Bytes(activityWapper.DataObj, activityWapper.ActivityDataType);
                await eventSender.ActivityExecuteStarted(activityId, Id, activityWapper.TimeOutSeconds, bytesContent, activityWapper.Order,DateTime.UtcNow);
                var result = await activityWapper.InvokeExecute();
                if (!result.IsSuccess)
                {
                    await eventSender.ActivityRevoked(activityId);
                    await CompensateActivity(result,currentExecuteOrder);
                    return result;
                }
                await eventSender.ActivityEnded(activityId, Encoding.UTF8.GetBytes(string.Empty));
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
                if (activityWapper.CancellationTokenSource.Token.IsCancellationRequested)
                {
                    var errors = exception.InnerException != null ? exception.InnerException.Message + exception.StackTrace : exception.Message + exception.StackTrace;
                    var result = new ActivityExecuteResult
                    {
                        IsSuccess = false,
                        Errors = errors
                    };
                    await eventSender.ActivityExecuteOvertime(activityId, Id, errors);
                    // 超时的时候 需要首先补偿这个超时的操作
                    return await CompensateActivity(result,currentExecuteOrder+1);
                }
                else
                {
                    var errors = exception.InnerException != null ? exception.InnerException.Message + exception.StackTrace : exception.Message + exception.StackTrace;
                    var result = new ActivityExecuteResult
                    {
                        IsSuccess = false,
                        Errors = errors
                    };
                    await eventSender.ActivityExecuteAborted(activityId, errors);
                    // 出错的时候 需要首先补偿这个出错的操作
                    return await CompensateActivity(result, currentExecuteOrder + 1);
                }
            }
        }

        private async Task<ActivityExecuteResult> CompensateActivity(ActivityExecuteResult result,int currentCompensateOrder)
        {
            this.currentCompensateOrder = currentCompensateOrder;
            currentExecuteOrder = -1;
            var compensateActivity = GetNextCompensateActivity();
            if (compensateActivity == null)
            {
                return result;
            }
            await RecursiveCompensateActivity(compensateActivity);
            return result;
        }
    }
}
