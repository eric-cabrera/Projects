namespace Assurity.AgentPortal.Accessors;

using Polly;

public interface IPollyPipelineAccessor
{
    IAsyncPolicy<HttpResponseMessage> GetRetryPolicy();
}
