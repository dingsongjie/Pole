using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.EventStorage.PostgreSql
{
    public class PostgreSqlOptions : EFOptions
    {
        public string ConnectionString { get; set; }
    }
}
