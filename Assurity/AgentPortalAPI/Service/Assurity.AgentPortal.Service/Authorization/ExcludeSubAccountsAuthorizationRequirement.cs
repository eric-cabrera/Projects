namespace Assurity.AgentPortal.Service.Authorization;

using Microsoft.AspNetCore.Authorization;

public class ExcludeSubAccountsAuthorizationRequirement(bool hasHistoricalAccess = false, bool allowHomeOffice = false) : IAuthorizationRequirement
{
    public bool HasHistoricalAccess { get; } = hasHistoricalAccess;

    /// <summary>
    /// True to allow non-impersonating home office users. False to deny non-impersonating home office users. Default: false.
    /// </summary>
    public bool AllowHomeOffice { get; } = allowHomeOffice;
}
