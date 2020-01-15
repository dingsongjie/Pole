using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            services.AddSingleton<IValidatorRegistrar, DefaultValidatorRegistrar>();
            services.AddSingleton<IValidatorErrorMessageHandler, DefaultValidatorErrorMessageHandler>();
            return services;
        }
        public static IServiceCollection AddGrpcRequestValidator(this IServiceCollection services,Assembly validatorAssembly ,ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {

            Action<ValidateOption> validateOptionConfig = validateOption => {
                validateOption.ValidatorAssembly = validatorAssembly;
            };

            services.Configure(validateOptionConfig);

            using (var serviceProvider= services.BuildServiceProvider())
            {
                var option = serviceProvider.GetRequiredService<IOptions<ValidateOption>>();
                var validatorRegistrar = serviceProvider.GetRequiredService<IValidatorRegistrar>();

                var validators = option.Value.ValidatorAssembly.GetTypes().Where(m => m.GetInterfaces().FirstOrDefault(t =>
                t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IValidator<>))!=null);
                foreach (var validator in validators)
                {
                    validatorRegistrar.Register(validator, services);
                }

                return services;
            }          
        }
    }
}
