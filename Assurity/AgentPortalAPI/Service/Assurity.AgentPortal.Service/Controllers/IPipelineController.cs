namespace Assurity.AgentPortal.Service.Controllers;

using Assurity.AgentPortal.Contracts.Integration;
using Assurity.AgentPortal.Managers.Integration;
using Assurity.AgentPortal.Managers.UserData;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.ExcludeTerminatedAgents)]
public class IPipelineController(
    ILogger<IPipelineController> logger,
    IConfigurationManager configurationManager,
    IIPipelineManager iPipelineManager,
    IUserDataManager userDataManager) : BaseController(configurationManager, logger)
{
    private IIPipelineManager IPipelineManager { get; } = iPipelineManager;

    private IUserDataManager UserDataManager { get; } = userDataManager;

    /// <summary>
    /// Get cerdentials for the agent to integrate with iPipeline.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="agentId"></param>
    /// <returns></returns>
    [HttpGet("Integration/IPipelineResponse")]
    [ProducesResponseType(typeof(IPipelineResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetIPipelineResponse(CancellationToken cancellationToken, string agentId = "")
    {
        try
        {
            var isHomeOfficeUser = HttpContext.IsHomeOfficeUser();

            if (string.IsNullOrEmpty(agentId))
            {
                agentId = HttpContext.GetAgentId();
            }

            var samlResponse = string.Empty;

            if (isHomeOfficeUser && string.IsNullOrEmpty(agentId))
            {
                samlResponse = IPipelineManager.GetBrowserPostSamlSignature();
            }
            else
            {
                if (string.IsNullOrEmpty(agentId))
                {
                    Logger.LogWarning("Access is denied.");
                    return Unauthorized();
                }

                var isSubaccount = HttpContext.IsSubaccount();
                if (isSubaccount)
                {
                    var additionalAgentIds = await UserDataManager.GetAdditionalAgentIds(agentId, cancellationToken);
                    if (!additionalAgentIds?.Contains(ConfigurationManager.IPipelineZanderAgentId) ?? false)
                    {
                        Logger.LogWarning("Access is denied for subaccounts.");
                        return Unauthorized();
                    }
                }

                samlResponse = await IPipelineManager.GetBrowserPostSamlSignature(agentId, cancellationToken);
            }

            if (samlResponse == null)
            {
                Logger.LogError("Failed to retrieve browser post SAML signature for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            var groupCode = IsZander(agentId, false) ?
                ConfigurationManager.IPipelineZanderCompanyId : ConfigurationManager.IPipelineAssurityCompanyId;
            var ipipelineTargetString = ConfigurationManager.IPipelineTargetString;
            var response = new IPipelineResponse
            {
                SAMLResponse = samlResponse,
                IPipelineConnectionString = ConfigurationManager.IPipelineConnectionString,
                IPipelineTargetString = ConfigurationManager.IPipelineTargetString + groupCode
            };

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    private bool IsZander(string agentId, bool noZander)
    {
        return agentId == ConfigurationManager.IPipelineZanderAgentId && !noZander;
    }
}