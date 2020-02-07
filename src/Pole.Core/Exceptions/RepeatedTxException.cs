using System;

namespace Pole.Core.Exceptions
{
    public class RepeatedTxException : Exception
    {
        public RepeatedTxException(string stateId, long transactionId, Type type) :
            base($"Grain type {type.FullName} with grainId {stateId} and transactionId {transactionId}")
        {
        }
    }
}
