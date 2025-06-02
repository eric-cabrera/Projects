namespace Assurity.AgentPortal.Service.IOCConfig.Extensions;

using Assurity.AgentPortal.Contracts.AgentContracts;

public static class HttpContextExtension
{
    public static bool IsHomeOfficeUser(this HttpContext context)
    {
        return (bool)(context.Items["IsHomeOfficeUser"] ?? false);
    }

    public static bool IsPingUser(this HttpContext context)
    {
        return (bool)(context.Items["IsPingUser"] ?? false);
    }

    public static bool IsSubaccount(this HttpContext context)
    {
        return (bool)(context.Items["IsSubaccount"] ?? false);
    }

    public static string GetAgentId(this HttpContext context)
    {
        var isSubaccount = context.IsSubaccount();
        if (isSubaccount)
        {
            return (string)(context.Items["ParentAgentId"] ?? string.Empty);
        }

        return (string)(context.Items["AgentId"] ?? string.Empty);
    }

    public static string GetAgentUsername(this HttpContext context)
    {
        return (string)(context.Items["AgentUsername"] ?? string.Empty);
    }

    public static string? GetAgentEmail(this HttpContext context)
    {
        return (string)(context.Items["AgentEmail"] ?? string.Empty);
    }

    public static bool AgentHasCorrectAccessLevel(this HttpContext context, bool hasHistoricalAccess)
    {
        var agentAcessLevel = (AccessLevel?)context.Items["AgentAccessLevel"];
        if (hasHistoricalAccess || (!hasHistoricalAccess && agentAcessLevel.HasValue && agentAcessLevel != AccessLevel.Historical))
        {
            return true;
        }

        return false;
    }

    public static bool AgentIsNotASubaccountOrSubaccountHasAccessToRole(this HttpContext context, string requiredRole)
    {
        var isSubaccount = context.IsSubaccount();
        if (isSubaccount)
        {
            return context.User != null && context.User.IsInRole(requiredRole);
        }

        return true;
    }
}
