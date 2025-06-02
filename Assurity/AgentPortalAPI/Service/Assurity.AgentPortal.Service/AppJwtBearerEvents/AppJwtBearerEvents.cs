#pragma warning disable SA1402 // File may only contain a single type.  Overriding for Middleware.
namespace Assurity.AgentPortal.Service.AppJwtBearerEvents;

using System.Security.Claims;
using Assurity.AgentPortal.Contracts.AgentContracts;
using Assurity.AgentPortal.Managers.UserData;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Primitives;

public class AppJwtBearerEvents(
    IConfigurationManager configurationManager,
    IUserDataManager userDataManager,
    ILogger<AppJwtBearerEvents> logger) : JwtBearerEvents
{
    private string issuer;

    private bool isHomeOfficeUser;

    private bool isPingUser;

    private string agentId;

    private bool isSubaccount;

    private string parentAgentId;

    private string username;

    private string email;

    private AccessLevel? accessLevel;

    private IConfigurationManager ConfigurationManager { get; } = configurationManager;

    private IUserDataManager UserDataManager { get; } = userDataManager;

    private ILogger<AppJwtBearerEvents> Logger { get; } = logger;

    public override async Task TokenValidated(TokenValidatedContext context)
    {
        Logger.LogInformation("TokenValidated");
        await GetClaimsAndHeaderValues(context);
        await base.TokenValidated(context);
        return;
    }

    private async Task GetClaimsAndHeaderValues(TokenValidatedContext context)
    {
        await InstantiateLocalPrivateVariables(context);

        AddContextItems(context);
    }

    private async Task InstantiateLocalPrivateVariables(TokenValidatedContext context)
    {
        issuer = context.Principal.Claims.FirstOrDefault()?.Issuer ?? string.Empty;
        Logger.LogDebug("Issuer retrieved from Claims and added to context items. {Issuer}", issuer);

        isHomeOfficeUser = issuer == configurationManager.AzureAdIssuer;
        Logger.LogDebug("IsHomeOfficeUser added to context items. {IsHomeOfficeUser}", isHomeOfficeUser);

        isPingUser = issuer == configurationManager.PingOneIssuer;
        Logger.LogDebug("IsPingUser added to context items. {IsPingUser}", isPingUser);

        agentId = GetAgentId(context);

        isSubaccount = agentId?.Equals("subaccount", StringComparison.OrdinalIgnoreCase) ?? false;

        if (isSubaccount)
        {
            parentAgentId = GetValueFromClaim(context.Principal, "parentAgentId") ?? string.Empty;
            Logger.LogDebug("Parent Agent Id retrieved from User Claims. {ParentAgentId}", parentAgentId);
        }

        username = GetAgentUsername(context);

        email = GetAgentEmail(context);

        accessLevel = await GetAgentAccessLevel(userDataManager);
    }

    private void AddContextItems(TokenValidatedContext context)
    {
        if (!context.HttpContext.Items.ContainsKey("IsHomeOfficeUser"))
        {
            context.HttpContext.Items.Add("IsHomeOfficeUser", isHomeOfficeUser);
            Logger.LogDebug("IsHomeOfficeUser added to context items. {IsHomeOfficeUser}", isHomeOfficeUser);
        }

        if (!context.HttpContext.Items.ContainsKey("IsPingUser"))
        {
            context.HttpContext.Items.Add("IsPingUser", isPingUser);
            Logger.LogDebug("IsPingUser added to context items. {IsPingUser}", isPingUser);
        }

        if (!context.HttpContext.Items.ContainsKey("IsSubaccount"))
        {
            context.HttpContext.Items.Add("IsSubaccount", isSubaccount);
            Logger.LogDebug("IsSubaccount added to context items. {IsSubaccount}", isSubaccount);
        }

        if (!context.HttpContext.Items.ContainsKey("AgentId"))
        {
            context.HttpContext.Items.Add("AgentId", agentId);
            Logger.LogDebug("AgentId added to context items. {AgentId}", agentId);
        }

        if (!context.HttpContext.Items.ContainsKey("ParentAgentId"))
        {
            context.HttpContext.Items.Add("ParentAgentId", parentAgentId);
            Logger.LogDebug("ParentAgentId added to context items. {ParentAgentId}", parentAgentId);
        }

        if (!context.HttpContext.Items.ContainsKey("AgentUsername"))
        {
            context.HttpContext.Items.Add("AgentUsername", username);
            Logger.LogDebug("AgentUsername added to context items. {AgentUsername}", username);
        }

        if (!context.HttpContext.Items.ContainsKey("AgentEmail"))
        {
            context.HttpContext.Items.Add("AgentEmail", email);
            Logger.LogDebug("AgentEmail added to context items. {AgentEmail}", email);
        }

        if (!context.HttpContext.Items.ContainsKey("AgentAccessLevel"))
        {
            context.HttpContext.Items.Add("AgentAccessLevel", accessLevel);
            Logger.LogDebug("AgentAccessLevel added to context items. {AgentAccessLevel}", accessLevel);
        }
    }

    private string? GetValueFromClaim(ClaimsPrincipal claims, string claimType)
    {
        var claimValue = claims?.FindFirst(claim => claim.Type == claimType)?.Value;
        Logger.LogDebug("{ClaimType} retrieved from User Claims. {ClaimValue}", claimType, claimValue);
        return claimValue;
    }

    private string? GetValueFromHeader(HttpContext context, string headerKey)
    {
        if (context.Request.Headers.TryGetValue(headerKey, out StringValues values))
        {
            var value = values.FirstOrDefault();
            Logger.LogDebug("{HeaderKey} retrieved from Headers. {HeaderValue}", headerKey, value);
            return value;
        }

        return null;
    }

    private string? GetAgentId(TokenValidatedContext context)
    {
        if (isHomeOfficeUser)
        {
            // Impersonation will put the agent id in the header.
            var agentId = GetValueFromHeader(context.HttpContext, "agent-id");
            if (agentId != null)
            {
                return agentId;
            }

            // Home office users shouldn't have access to reports unless they're impersonating as they don't have any data.
            return null;
        }

        if (isPingUser)
        {
            return GetValueFromClaim(context.Principal, "agentId");
        }

        return null;
    }

    private string? GetAgentUsername(TokenValidatedContext context)
    {
        if (isHomeOfficeUser)
        {
            return GetValueFromHeader(context.HttpContext, "agent-username");
        }

        if (isPingUser)
        {
            return GetValueFromClaim(context.Principal, "username");
        }

        return null;
    }

    private string? GetAgentEmail(TokenValidatedContext context)
    {
        if (isHomeOfficeUser)
        {
            return GetValueFromHeader(context.HttpContext, "agent-email");
        }

        if (isPingUser)
        {
            return GetValueFromClaim(context.Principal, "email");
        }

        return null;
    }

    private async Task<AccessLevel?> GetAgentAccessLevel(IUserDataManager userDataManager)
    {
        if (isSubaccount && !string.IsNullOrEmpty(parentAgentId))
        {
            Logger.LogDebug("Agent is a sub account, using parent agent id {ParentAgentId} to get agent access level.", parentAgentId);
            return await userDataManager.GetAgentAccess(parentAgentId);
        }

        if (!string.IsNullOrEmpty(agentId))
        {
            Logger.LogDebug("Agent is not a sub account, using agent id {AgentId} to get agent access level.", agentId);
            return await userDataManager.GetAgentAccess(agentId);
        }

        return null;
    }
}
