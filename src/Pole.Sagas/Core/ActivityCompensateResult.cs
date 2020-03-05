using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Core
{
    public class ActivityCompensateResult
    {
        /// <summary>
        ///  If not success , this activity will be aborted , and current saga will compensate all previous activities
        /// </summary>
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
