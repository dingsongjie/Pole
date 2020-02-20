using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Core.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UsePole(this IApplicationBuilder applicationBuilder)
        {
            Startup.StartRay(applicationBuilder.ApplicationServices);
            return applicationBuilder;
        }
    }
}
