using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pole.Core.Utils;
using Pole.Grpc.Validation;
using Pole.Grpc.Validation.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionValidationExtensions
    {
        public static IServiceCollection AddGrpcValidation(this IServiceCollection services)
        {
            services.AddSingleton<IValidatorProvider, DefaultValidatorProvider>();
            services.AddSingleton<IValidatorRegistrar, ValidatorRegistrar>();
            services.AddSingleton<IValidatorErrorMessageHandler, DefaultValidatorErrorMessageHandler>();
            return services;
        }
        public static IServiceCollection AddGrpcRequestValidator(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                var validatorRegistrar = serviceProvider.GetRequiredService<IValidatorRegistrar>();

                foreach (var assembly in AssemblyHelper.GetAssemblies(serviceProvider.GetService<ILogger<ValidatorRegistrar>>()))
                {
                    var validators = assembly.GetTypes().Where(m => m.GetInterfaces().FirstOrDefault(t =>t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IValidator<>)) != null);
                    foreach (var validator in validators)
                    {
                        validatorRegistrar.Register(validator, services);
                    }
                }
                return services;
            }
        }
    }
}
