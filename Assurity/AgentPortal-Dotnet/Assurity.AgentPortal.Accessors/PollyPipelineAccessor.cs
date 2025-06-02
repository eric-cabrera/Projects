namespace Assurity.AgentPortal.Accessors;

using System;
using System.Net.Http;
using global::Polly;
using global::Polly.Extensions.Http;

public class PollyPipelineAccessor : IPollyPipelineAccessor
{
    public IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
