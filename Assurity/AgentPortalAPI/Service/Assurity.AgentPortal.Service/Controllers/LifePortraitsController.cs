namespace Assurity.AgentPortal.Service.Controllers;

using Assurity.AgentPortal.Managers.Integration;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Elastic.Clients.Elasticsearch.MachineLearning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Nest;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.ExcludeTerminatedAgents)]
public class LifePortraitsController(
    ILogger<LifePortraitsController> logger,
    IConfigurationManager configurationManager,
    ILifePortraitsManager lifePortraitsManager) : BaseController(configurationManager, logger)
{
    private ILifePortraitsManager LifePortraitsManager { get; } = lifePortraitsManager;

    /// <summary>
    /// Get redirectURL for the agent to integrate with LifePortraits.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("Integration/RedirectURL")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetRedirectURL(CancellationToken cancellationToken)
    {
        try
        {
            var lifePortraitsURL = string.Empty;
            var agentId = HttpContext.GetAgentId();
            var username = HttpContext.GetAgentUsername();
            var isHomeOfficeUser = HttpContext.IsHomeOfficeUser();

            if (isHomeOfficeUser && string.IsNullOrEmpty(username))
            {
                username = User.Claims.Where(x => x.Type == "preferred_username").FirstOrDefault()?.Value;
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                Logger.LogError("Failed to retrieve username for life portraits.");
                return ErrorResponse();
            }

            if (string.IsNullOrWhiteSpace(agentId) && !isHomeOfficeUser)
            {
                Logger.LogWarning("Access is denied.");
                return Unauthorized();
            }
            else if (!string.IsNullOrWhiteSpace(agentId))
            {
                var credentials = await LifePortraitsManager.GetCredentialsForLifePortraits(agentId, username, cancellationToken);

                if (credentials == null)
                {
                    Logger.LogError("Failed to retrieve credentials for life portraits for agent {AgentId}", agentId);
                    return ErrorResponse();
                }

                lifePortraitsURL = await LifePortraitsManager.GetLifePortraitsURL(credentials);
                if (string.IsNullOrEmpty(lifePortraitsURL))
                {
                    Logger.LogError("Failed to retrieve URL for life portraits for agent {AgentId}", agentId);
                    return ErrorResponse();
                }
            }
            else if (isHomeOfficeUser)
            {
                var homeOfficeCredentials = LifePortraitsManager.GetCredentialsForLifePortraitsForHomeOfficeUser(username);

                if (homeOfficeCredentials == null)
                {
                    Logger.LogError("Failed to retrieve credentials for life portraits for homeoffice user");
                    return ErrorResponse();
                }

                lifePortraitsURL = await LifePortraitsManager.GetLifePortraitsURL(homeOfficeCredentials.Result);
                if (string.IsNullOrEmpty(lifePortraitsURL))
                {
                    Logger.LogError("Failed to retrieve URL for life portraits for homeoffice user");
                    return ErrorResponse();
                }
            }

            return Ok(lifePortraitsURL);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}
