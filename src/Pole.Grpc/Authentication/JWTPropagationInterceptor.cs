using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pole.Grpc.Authentication
{
    public class JWTPropagationInterceptor : Interceptor
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private const string Authorization = "Authorization";
        public JWTPropagationInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            if (httpContextAccessor.HttpContext == null)
            {
                return base.AsyncUnaryCall(request, context, continuation);
            }
            var httpContext = httpContextAccessor.HttpContext;
            if (!httpContext.Request.Headers.ContainsKey(Authorization))
            {
                return base.AsyncUnaryCall(request, context, continuation);
            }
            var tokenStringValues = httpContext.Request.Headers[Authorization];
            var token = tokenStringValues.FirstOrDefault();
            var entry = new Metadata.Entry(Authorization, token);
            var metadata = new Metadata();
            metadata.Add(entry);
            var newOption = context.Options.WithHeaders(metadata);
            var newContext = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, newOption);
            return base.AsyncUnaryCall(request, newContext, continuation);
        }
    }
}
