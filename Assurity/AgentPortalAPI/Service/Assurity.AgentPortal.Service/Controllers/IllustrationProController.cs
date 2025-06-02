namespace Assurity.AgentPortal.Service.Controllers;

using Assurity.AgentPortal.Managers.Integration;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.ExcludeTerminatedAgents)]
public class IllustrationProController(
    ILogger<IllustrationProController> logger,
    IConfigurationManager configurationManager,
    IIllustrationProManager illustrationProManager) : BaseController(configurationManager, logger)
{
    private IIllustrationProManager IllustrationProManager { get; } = illustrationProManager;

    /// <summary>
    /// Get cerdentials for the agent to integrate with IllustrationPro.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("Integration/AccountId")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetAccountId(CancellationToken cancellationToken)
    {
        try
        {
            var accountId = string.Empty;

            var agentId = HttpContext.GetAgentId();
            var username = HttpContext.GetAgentUsername();
            var isHomeOfficeUser = HttpContext.IsHomeOfficeUser();

            if (isHomeOfficeUser && string.IsNullOrEmpty(username))
            {
                username = User.Claims.Where(x => x.Type == "preferred_username").FirstOrDefault()?.Value;
            }

            if (username?.IndexOf("@") > 0)
            {
                username = username?.Substring(0, username.IndexOf("@"));
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                Logger.LogError("Failed to retrieve username for Illustrations.");
                return ErrorResponse();
            }

            if (string.IsNullOrWhiteSpace(agentId) && !isHomeOfficeUser)
            {
                Logger.LogWarning("Access is denied.");
                return Unauthorized();
            }
            else if (!string.IsNullOrWhiteSpace(agentId))
            {
                var credentials = await IllustrationProManager.GetCredentialsForIllustrationPro(agentId, username, cancellationToken);

                if (credentials == null)
                {
                    Logger.LogError("Failed to retrieve credentials for Illustration Pro for agent {AgentId} with the user name {Username}", agentId, username);
                    return ErrorResponse();
                }

                accountId = await IllustrationProManager.GetIllustrationProAccountId(credentials);

                if (accountId == null)
                {
                    Logger.LogError("Failed to retrieve Account Id for Illustration Pro for agent {AgentId} with the user name {Username}", agentId, username);
                    return ErrorResponse();
                }
            }
            else if (isHomeOfficeUser)
            {
                var homeOfficeCredentials = IllustrationProManager.GetCredentialsForIllustrationProForHomeOfficeUser(username);

                if (homeOfficeCredentials == null)
                {
                    Logger.LogError("Failed to retrieve credentials for life portraits for homeoffice user");
                    return ErrorResponse();
                }

                accountId = await IllustrationProManager.GetIllustrationProAccountId(homeOfficeCredentials);
                if (string.IsNullOrEmpty(accountId))
                {
                    Logger.LogError("Failed to retrieve URL for life portraits for homeoffice user");
                    return ErrorResponse();
                }
            }

            return Ok(accountId);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}