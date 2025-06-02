namespace Assurity.AgentPortal.Service.Controllers;

using System.Net;
using Assurity.AgentPortal.Contracts.Alerts;
using Assurity.AgentPortal.Managers.Alerts;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.ExcludeSubAccounts)]
public class AlertsController(
    ILogger<AlertsController> logger,
    IConfigurationManager configurationManager,
    IAlertsManager alertsManager) : BaseController(configurationManager, logger)
{
    private IAlertsManager AlertsManager { get; } = alertsManager;

    [HttpGet("GetDistributionEmails", Name = nameof(GetDistributionEmails))]
    [ProducesResponseType(typeof(List<DistributionList>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetDistributionEmails(CancellationToken cancellationToken)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var distributionEmails = await AlertsManager.GetDistributionEmailsByAgentId(agentId, cancellationToken);
            if (distributionEmails == null)
            {
                Logger.LogError("Failed to retrieve distribution emails for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(distributionEmails);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpPost("AddDistributionEmail", Name = nameof(AddDistributionEmail))]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddDistributionEmail(
        [FromQuery] string email,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return ErrorResponse("Email is required.", HttpStatusCode.BadRequest, StatusCodes.Status400BadRequest);
        }

        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            await AlertsManager.AddDistributionEmail(agentId, email, cancellationToken);

            return Ok();
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpDelete("DeleteDistributionEmail", Name = nameof(DeleteDistributionEmail))]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteDistributionEmail(
        [FromQuery] int emailId,
        CancellationToken cancellationToken)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            await AlertsManager.DeleteDistributionEmail(emailId, agentId, cancellationToken);

            return Ok();
        }
        catch (KeyNotFoundException exception)
        {
            Logger.LogError(exception, "Unable to delete distribution list with the distribution list ID {emailId} and Agent ID {agentId}; it was not found.", emailId, agentId);
            return StatusCode(StatusCodes.Status400BadRequest);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("GetAlertPreferences", Name = nameof(GetAlertPreferences))]
    [ProducesResponseType(typeof(AlertPreferences), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAlertPreferences(CancellationToken cancellationToken)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var alertPreferences = await AlertsManager.GetAlertPreferencesByAgentId(agentId, cancellationToken);
            if (alertPreferences == null)
            {
                Logger.LogError("Failed to retrieve alert preferences for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(alertPreferences);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpPost("AddOrUpdateAlertPreferences", Name = nameof(AddOrUpdateAlertPreferences))]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddOrUpdateAlertPreferences(
        [FromBody] AlertPreferences alertPreferences,
        CancellationToken cancellationToken)
    {
        if (alertPreferences == null ||
            !alertPreferences.DisableAll.HasValue ||
            !alertPreferences.SelfAdd.HasValue ||
            !alertPreferences.SelfMet.HasValue ||
            !alertPreferences.SelfOutstanding.HasValue ||
            !alertPreferences.HierarchyAdd.HasValue ||
            !alertPreferences.HierarchyMet.HasValue ||
            !alertPreferences.HierarchyOutstanding.HasValue)
        {
            return BadRequestResponse("Alert preferences and all of its parameters are required.");
        }

        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            await AlertsManager.AddOrUpdateAlertPreferences(agentId, alertPreferences, cancellationToken);

            return Ok();
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpDelete("DeleteAlertPreferences", Name = nameof(DeleteAlertPreferences))]
    [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAlertPreferences(CancellationToken cancellationToken)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            await AlertsManager.DeleteAlertPreferences(agentId, cancellationToken);

            return Ok();
        }
        catch (KeyNotFoundException exception)
        {
            Logger.LogError(exception, "Unable to delete distribution master with the agent id {agentId}; it was not found.", agentId);
            return BadRequestResponse();
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}
