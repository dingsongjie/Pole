using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.EventStorage.PostgreSql
{
    public class EFOptions
    {
        public const string DefaultSchema = "pole";
        public const string DefaultTable = "Events";
        /// <summary>
        /// Gets or sets the schema to use when creating database objects.
        /// Default is <see cref="DefaultSchema" />.
        /// </summary>
        public string Schema { get; set; } = DefaultSchema;
        public string TableName { get; set; } = DefaultTable;

        internal Type DbContextType { get; set; }
    }
}
