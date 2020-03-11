using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Sagas.Server
{
    public class PoleSagasServerOption
    {
        public int NotEndedSagasFetchIntervalSeconds { get; set; } = 30;
        public int GetSagasGrpcStreamingResponseDelaySeconds { get; set; } = 20;
        public int ExpiredDataBulkDeleteIntervalSeconds { get; set; } = 10*60;
        public int ExpiredDataDeleteBatchCount { get; set; } = 1000;
        public int ExpiredDataPreBulkDeleteDelaySeconds { get; set; } = 3;
    }
}
