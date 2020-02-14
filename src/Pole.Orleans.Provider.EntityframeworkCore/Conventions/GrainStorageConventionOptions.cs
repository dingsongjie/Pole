using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Orleans.Provider.EntityframeworkCore.Conventions
{
    public class GrainStorageConventionOptions
    {
        public string DefaultGrainKeyPropertyName { get; set; } = "Id";

        public string DefaultGrainKeyExtPropertyName { get; set; } = "KeyExt";

        public string DefaultPersistenceCheckPropertyName { get; set; } = "Id";
    }
}
