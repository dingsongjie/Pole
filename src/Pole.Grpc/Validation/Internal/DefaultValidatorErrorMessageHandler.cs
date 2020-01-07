using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using Grpc.Core;

namespace Pole.Grpc.Validation.Internal
{
    class DefaultValidatorErrorMessageHandler : IValidatorErrorMessageHandler
    {
        public Task HandleAsync(IList<ValidationFailure> failures, ServerCallContext serverCallContext)
        {
            var requestPath=  serverCallContext.GetHttpContext().Request.Path.Value;

            var errors = failures
                .Select(f => $"Property {f.PropertyName} failed validation. Error was {f.ErrorMessage}. Request path was {requestPath}")
                .ToList();
            var message = string.Join("\n", errors);

            serverCallContext.Status = new Status(StatusCode.InvalidArgument, message);
            return Task.FromResult(1);
        }
    }
}
