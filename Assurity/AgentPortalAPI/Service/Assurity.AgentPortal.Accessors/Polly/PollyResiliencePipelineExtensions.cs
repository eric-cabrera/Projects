namespace Assurity.AgentPortal.Accessors.Polly;

using System.Net;
using global::Polly;
using global::Polly.Retry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class PollyResiliencePipelineExtensions
{
    public static void AddHttpClientRetryPipeline(
        this IServiceCollection services)
    {
        HttpStatusCode[] httpStatusCodesWorthRetrying =
        {
                   HttpStatusCode.RequestTimeout,     // 408
                   HttpStatusCode.BadGateway,         // 502
                   HttpStatusCode.ServiceUnavailable, // 503
                   HttpStatusCode.GatewayTimeout,     // 504
                   HttpStatusCode.InternalServerError // 500
        };

        services.AddResiliencePipeline<string, HttpResponseMessage>(PollyPipelineKeys.HttpRetryPipeline, (builder, context) =>
            builder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(1),
                UseJitter = true,
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .HandleResult(response => !response.IsSuccessStatusCode && httpStatusCodesWorthRetrying.Contains(response.StatusCode))
                    .Handle<HttpRequestException>(),
                OnRetry = (args) =>
                {
                    var logger = context.ServiceProvider.GetService<ILogger<object>>();
                    if (logger != null)
                    {
                        logger.LogWarning("Retrying API call Attempt: {Attempt}", args.AttemptNumber);
                    }

                    return default;
                }
            }));
    }
}