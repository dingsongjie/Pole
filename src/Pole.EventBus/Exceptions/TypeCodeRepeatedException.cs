using System;

namespace Pole.EventBus.Exceptions
{
    public class TypeCodeRepeatedException : Exception
    {
        public TypeCodeRepeatedException(string typeName, string typeFullName) : base($"Type named {typeName} was repeated of {typeFullName}.")
        {
        }
    }
}
