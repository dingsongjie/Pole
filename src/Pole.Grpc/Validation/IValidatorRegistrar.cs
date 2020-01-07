using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Grpc.Validation
{
    public interface IValidatorRegistrar
    {
        Task Register(Type validatorType,IServiceCollection services, ServiceLifetime lifetime= ServiceLifetime.Singleton);
    }
}
