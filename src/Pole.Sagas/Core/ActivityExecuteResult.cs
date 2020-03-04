using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public class ActivityExecuteResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
