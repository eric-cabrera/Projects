namespace Assurity.AgentPortal.Utilities.Logging
{
    using Microsoft.AspNetCore.Http;

    public interface IHttpRequestMessageValuesProvider
    {
        Guid ExtractGuid(IHeaderDictionary requestHeaders);
    }
}
