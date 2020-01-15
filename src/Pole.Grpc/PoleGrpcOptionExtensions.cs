using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pole.Application;
using Pole.Domain;

namespace Pole.Grpc
{
    public static class PoleGrpcOptionExtensions
    {
        public static PoleGrpcOptions AddPoleApplication(this PoleGrpcOptions poleGrpcOptions)
        {
            poleGrpcOptions.Services.AddPoleApplication(options => {
                options.AutoInjectionCommandHandlersAndDomainEventHandlers();
                if (poleGrpcOptions.AutoInject)
                {
                    options.AutoInjectionDependency();
                }
            }, poleGrpcOptions.ApplicationAssemblies.ToArray());

            return poleGrpcOptions;
        }
        public static PoleGrpcOptions AddPoleDomain(this PoleGrpcOptions poleGrpcOptions)
        {
            poleGrpcOptions.Services.AddPoleDomain();
            return poleGrpcOptions;
        }
    }
}
