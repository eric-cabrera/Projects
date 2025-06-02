#pragma warning disable SA1402 // File may only contain a single type.  Overriding for Middleware.
namespace Assurity.AgentPortal.Service.Middleware
{
    using Assurity.AgentPortal.Service.Helpers;
    using Serilog.Context;

    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }

    public class LoggingMiddleware
    {
        private readonly RequestDelegate next;

        public LoggingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task InvokeAsync(HttpContext ctx)
        {
            using (LogContext.PushProperty(LogHelper.Issuer, ctx.User.Claims.FirstOrDefault()?.Issuer))
            using (LogContext.PushProperty(LogHelper.UserId, ctx.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value))
            using (LogContext.PushProperty(LogHelper.Username, ctx.User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value))
            using (LogContext.PushProperty(LogHelper.Email, ctx.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value))
            {
                return next(ctx);
            }
        }
    }
}