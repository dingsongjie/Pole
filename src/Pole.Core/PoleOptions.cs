using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core
{
    public class PoleOptions
    {
        public IServiceCollection Services { get; private set; }
    }
}
