using Microsoft.AspNetCore.Builder;
using Pole.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace  Microsoft.AspNetCore.Builder
{
    public static class PoleApplicationBuilderExtensions
    {
        public static IApplicationBuilder UsePole(this IApplicationBuilder applicationBuilder)
        {
            StartupBuilder.StartPole(applicationBuilder.ApplicationServices).GetAwaiter().GetResult();
            return applicationBuilder;
        }
    }
}
