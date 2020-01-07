using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Grpc.Validation.Internal
{
    class DefaultValidatorRegistrar : IValidatorRegistrar
    {
        public Task Register(Type validatorType, IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var messageType = validatorType.GetInterfaces().FirstOrDefault(t =>
                t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IValidator<>)).GetGenericArguments().First();
            var serviceType = typeof(IValidator<>).MakeGenericType(messageType);

            services.Add(new ServiceDescriptor(serviceType, validatorType, lifetime));
            return Task.FromResult(1);
        }
    }
}
