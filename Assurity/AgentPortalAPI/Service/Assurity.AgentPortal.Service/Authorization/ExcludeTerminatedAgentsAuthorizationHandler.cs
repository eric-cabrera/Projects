namespace Assurity.AgentPortal.Service.Authorization;

using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

public class ExcludeTerminatedAgentsAuthorizationHandler(ILogger<ExcludeTerminatedAgentsAuthorizationHandler> logger) : AuthorizationHandler<ExcludeTerminatedAgentsAuthorizationRequirement>
{
    private ILogger<ExcludeTerminatedAgentsAuthorizationHandler> Logger { get; } = logger;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExcludeTerminatedAgentsAuthorizationRequirement requirement)
    {
        if (context.Resource is HttpContext httpContext)
        {
            var agentId = httpContext.GetAgentId();
            if (string.IsNullOrEmpty(agentId))
            {
                if (requirement.AllowHomeOffice && httpContext.IsHomeOfficeUser())
                {
                    context.Succeed(requirement);
                }
                else
                {
                    Logger.LogError("Agent Id retrieved from the header or claim was null. Authentication requirement failed. {Requirement}", requirement);
                }

                return Task.CompletedTask;
            }

            var agentHasCorrectAcessLevel = httpContext.AgentHasCorrectAccessLevel(requirement.HasHistoricalAccess);
            if (!agentHasCorrectAcessLevel)
            {
                Logger.LogDebug(
                    "Agent {AgentId} has the incorrect access level. Authentication requirement failed. Report HasHistoricalAccess: {Requirement}",
                    agentId,
                    requirement.HasHistoricalAccess);

                return Task.CompletedTask;
            }

            if (httpContext.IsHomeOfficeUser())
            {
                Logger.LogDebug("Agent {AgentId} is a Home Office User. Authentication requirement succeeded.", agentId);
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            else if (httpContext.IsPingUser())
            {
                Logger.LogDebug("Agent {AgentId} Authentication requirement succeeded.", agentId);
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            Logger.LogError("Invalid issuer.  Authentication requirement failed.  {Requirement}", requirement);
            return Task.CompletedTask;
        }

        Logger.LogError("Invalid context.");
        return Task.CompletedTask;
    }
}
