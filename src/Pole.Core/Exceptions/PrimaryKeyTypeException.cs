using System;

namespace Pole.Core.Exceptions
{
    public class PrimaryKeyTypeException : Exception
    {
        public PrimaryKeyTypeException(string name) : base(name)
        {

        }
    }
}
