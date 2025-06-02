namespace Assurity.AgentPortal.Service.Authorization;

using Microsoft.AspNetCore.Authorization;

public class AssurityAuthorizationRequirement(string role, bool hasHistoricalAccess = false) : IAuthorizationRequirement
{
    public string RequiredRole { get; } = role;

    public bool HasHistoricalAccess { get; } = hasHistoricalAccess;
}
