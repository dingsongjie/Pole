using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Server
{
    public class PoleSagasServerOption
    {
        public int NotEndedSagasFetchIntervalSeconds { get; set; } = 10;
        public int ExpiredDataBulkDeleteIntervalSeconds { get; set; } = 10*60;
        public int ExpiredDataDeleteBatchCount { get; set; } = 1000;
        public int ExpiredDataPreBulkDeleteDelaySeconds { get; set; } = 3;
    }
}
