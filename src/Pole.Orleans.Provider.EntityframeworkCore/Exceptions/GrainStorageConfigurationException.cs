using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Orleans.Provider.EntityframeworkCore
{
    // todo: Use for configuration errors
    public class GrainStorageConfigurationException : Exception
    {
        public GrainStorageConfigurationException()
        {
        }

        public GrainStorageConfigurationException(string message) : base(message)
        {
        }

        public GrainStorageConfigurationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
