using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Pole.Grpc.Validation.Internal
{
    class DefaultValidatorProvider : IValidatorProvider
    {
        private readonly IServiceProvider _provider;

        public DefaultValidatorProvider(IServiceProvider provider)
        {
            _provider = provider;
        }

        public bool TryGetValidator<TRequest>(out IValidator<TRequest> result) where TRequest : class
        {
            result = _provider.GetService<IValidator<TRequest>>();
            return result != null;
        }
    }
}
