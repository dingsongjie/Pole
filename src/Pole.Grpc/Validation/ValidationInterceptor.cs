using Grpc.Core;
using Grpc.Core.Interceptors;
using Pole.Grpc.Validation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Pole.Grpc.Validation
{
    public class ValidationInterceptor : Interceptor
    {
        private readonly IValidatorProvider _provider;
        private readonly IValidatorErrorMessageHandler _handler;
        public ValidationInterceptor(IValidatorProvider provider, IValidatorErrorMessageHandler handler)
        {
            _provider = provider;
            _handler = handler;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            if (_provider.TryGetValidator<TRequest>(out var validator))
            {
                var results = await validator.ValidateAsync(request);

                if (!results.IsValid)
                {
                    await _handler.HandleAsync(results.Errors, context);

                    var response= Expression.Lambda<Func<TResponse>>(Expression.Block(typeof(TResponse), Expression.New(typeof(TResponse)))).Compile();
                    return response();
                }
            }

            return await continuation(request, context);
        }
    }
}
