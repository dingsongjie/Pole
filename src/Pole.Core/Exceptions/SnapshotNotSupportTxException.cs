using System;

namespace Pole.Core.Exceptions
{
    public class SnapshotNotSupportTxException : Exception
    {
        public SnapshotNotSupportTxException(Type type) : base(type.FullName)
        {
        }
    }
}
