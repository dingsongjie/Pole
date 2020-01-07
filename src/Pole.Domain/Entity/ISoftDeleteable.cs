using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Domain
{
    public interface ISoftDeleteable
    {
        bool IsDelete { get; set; }
    }
}
