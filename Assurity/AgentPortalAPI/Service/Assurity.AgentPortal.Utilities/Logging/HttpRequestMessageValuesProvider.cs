namespace Assurity.AgentPortal.Utilities.Logging
{
    using Microsoft.AspNetCore.Http;

    public class HttpRequestMessageValuesProvider : IHttpRequestMessageValuesProvider
    {
        private const string GuidHeader = "Guid";

        public Guid ExtractGuid(IHeaderDictionary requestHeaders)
        {
            var guid = Guid.NewGuid();

            if (requestHeaders == null || !requestHeaders.Any())
            {
                return guid;
            }

            var guidHeaders = requestHeaders[GuidHeader];

            if (!string.IsNullOrEmpty(guidHeaders))
            {
                Guid.TryParse(guidHeaders, out guid);
            }

            return guid;
        }
    }
}
