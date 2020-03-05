using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public class ActivityExecuteResult
    {
        public bool IsSuccess { get; set; }
        public object Result { get; set; }
        public string Errors { get; set; }
        public static ActivityExecuteResult Success = new ActivityExecuteResult
        {
            IsSuccess = true
        };

        public static implicit operator SagaResult(ActivityExecuteResult activity)
        {
            return new SagaResult
            {
                IsSuccess = activity.IsSuccess,
                Result = activity.Result,
                HasException = !string.IsNullOrEmpty(activity.Errors),
                ExceptionMessages = activity.Errors
            };
        }
        
    }
}
