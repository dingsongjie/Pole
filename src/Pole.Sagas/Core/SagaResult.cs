using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public class SagaResult
    {
        public bool IsSuccess { get; set; }
        public bool HasException { get; set; }
        public object Result { get; set; }
        public string ExceptionMessages { get; set; } = string.Empty;

        public static SagaResult SuccessResult = new SagaResult
        {
            IsSuccess = true,
            Result = default
        };
    }
}
