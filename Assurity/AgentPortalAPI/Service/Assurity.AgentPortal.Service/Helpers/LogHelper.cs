namespace Assurity.AgentPortal.Service.Helpers;

using Serilog;
using Serilog.Events;

public static class LogHelper
{
    public const string UserId = "UserID";
    public const string Username = "Username";
    public const string Issuer = "Issuer";
    public const string Email = "Email";

    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;

        // Set all the common properties available for every request
        diagnosticContext.Set("Host", request.Host);
        diagnosticContext.Set("Protocol", request.Protocol);
        diagnosticContext.Set("Scheme", request.Scheme);

        // Only set it if available. You're not sending sensitive data in a querystring right?!
        if (request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", request.QueryString.Value);
        }

        // Set the content-type of the Response at this point
        diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

        // Retrieve the IEndpointFeature selected for the request
        var endpoint = httpContext.GetEndpoint();
        if (endpoint is object)
        {
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
        }

        if (httpContext.User?.Identity?.IsAuthenticated ?? false)
        {
            diagnosticContext.Set(Issuer, httpContext.User.Claims.FirstOrDefault()?.Issuer);
            diagnosticContext.Set(UserId, httpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value);
            diagnosticContext.Set(Username, httpContext.User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value);
            diagnosticContext.Set(Email, httpContext.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value);
        }
    }

    public static LogEventLevel ExcludeCustomPaths(HttpContext ctx, double x, Exception? ex) =>
        ex != null
            ? LogEventLevel.Error
            : ctx.Response.StatusCode > 499
                ? LogEventLevel.Error
                : IsIgnoredPath(ctx)
                    ? LogEventLevel.Verbose
                    : LogEventLevel.Information;

    private static bool IsIgnoredPath(HttpContext ctx)
    {
        string[] ignoredPaths =
        {
            "/API/heartbeat/", "/heartbeat/"
        };

        var path = ctx.Request.Path.Value;

        return path != null && ignoredPaths.Any(x => path.StartsWith(x));
    }
}