using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pole.Grpc.Validation
{
    public interface IValidatorProvider
    {
        bool TryGetValidator<TRequest>(out IValidator<TRequest> result) where TRequest : class;
    }
}
