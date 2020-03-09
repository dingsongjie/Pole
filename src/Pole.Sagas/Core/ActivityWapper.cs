using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pole.Sagas.Core
{
    public class ActivityWapper
    {
        public string Id { get; set; }
        public Type ActivityType { get; set; }
        public Type ActivityDataType { get; set; }
        public object DataObj { get; set; }
        public int Order { get; set; }
        public ActivityStatus ActivityStatus { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public int TimeOutSeconds { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public Task<ActivityExecuteResult> InvokeExecute()
        {
            var activityObjParams = Expression.Parameter(typeof(object), "activity");
            var activityParams = Expression.Convert(activityObjParams, ActivityType);
            var dataObjParams = Expression.Parameter(typeof(object), "data");
            var dataParams = Expression.Convert(dataObjParams, ActivityDataType);
            var cancellationTokenParams = Expression.Parameter(typeof(CancellationToken), "ct");
            var method = ActivityType.GetMethod("Execute", new Type[] { ActivityDataType, typeof(CancellationToken) });
            var body = Expression.Call(activityParams, method, dataParams, cancellationTokenParams);
            var func = Expression.Lambda<Func<object, object, CancellationToken, Task<ActivityExecuteResult>>>(body, true, activityObjParams, dataObjParams, cancellationTokenParams).Compile();

            using (var scope = ServiceProvider.CreateScope())
            {
                var activity = scope.ServiceProvider.GetRequiredService(ActivityType);
                return func(activity, DataObj, CancellationTokenSource.Token);
            }
        }
        public Task InvokeCompensate()
        {
            var activityObjParams = Expression.Parameter(typeof(object), "activity");
            var activityParams = Expression.Convert(activityObjParams, ActivityType);
            var dataObjParams = Expression.Parameter(typeof(object), "data");
            var dataParams = Expression.Convert(dataObjParams, ActivityDataType);
            var cancellationTokenParams = Expression.Parameter(typeof(CancellationToken), "ct");
            var method = ActivityType.GetMethod("Compensate", new Type[] { ActivityDataType, typeof(CancellationToken) });
            var body = Expression.Call(activityParams, method, dataParams, cancellationTokenParams);
            var func = Expression.Lambda<Func<object, object, CancellationToken, Task>>(body, activityObjParams, dataObjParams, cancellationTokenParams).Compile();

            using (var scope = ServiceProvider.CreateScope())
            {
                var activity = scope.ServiceProvider.GetRequiredService(ActivityType);
                return func(activity, DataObj, CancellationTokenSource.Token);
            }
        }
    }
}
