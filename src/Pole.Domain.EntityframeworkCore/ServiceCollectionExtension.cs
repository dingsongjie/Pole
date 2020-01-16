using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pole.Application;
using Pole.Domain.EntityframeworkCore;
using Pole.Domain.EntityframeworkCore.UnitOfWork;
using Pole.Domain.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PoleApplicationOptionsExtension
    {
        public static PoleOptions AddPoleEntityFrameworkCoreDomain(this PoleOptions options) 
        {
            options.Services.AddScoped<IUnitOfWorkManager, EntityFrameworkCoreUnitOfWorkManager>();
            return options;
        }
    }
}
