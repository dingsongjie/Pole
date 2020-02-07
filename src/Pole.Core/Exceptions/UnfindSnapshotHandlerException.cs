using System;

namespace Pole.Core.Exceptions
{
    public class UnfindSnapshotHandlerException : Exception
    {
        public UnfindSnapshotHandlerException(Type grainType) : base(grainType.FullName)
        {

        }
    }
}
