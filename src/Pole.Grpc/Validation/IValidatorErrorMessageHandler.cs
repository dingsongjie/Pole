using FluentValidation.Results;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Grpc.Validation
{
    public interface IValidatorErrorMessageHandler
    {
        Task HandleAsync(IList<ValidationFailure> failures, ServerCallContext serverCallContext);
    }
}
