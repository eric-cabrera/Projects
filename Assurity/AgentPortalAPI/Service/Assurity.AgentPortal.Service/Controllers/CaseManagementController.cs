namespace Assurity.AgentPortal.Service.Controllers;

using System.Threading;
using Assurity.AgentPortal.Contracts.CaseManagement;
using Assurity.AgentPortal.Managers.CaseManagement;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.CaseManagement)]
public class CaseManagementController(
    ILogger<CaseManagementController> logger,
    IConfigurationManager configurationManager,
    ICaseManagementManager caseManagementManager) : BaseController(configurationManager, logger)
{
    private ICaseManagementManager CaseManagementManager { get; } = caseManagementManager;

    [HttpGet("Cases")]
    [ProducesResponseType(typeof(CaseManagementResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetCases([FromQuery] CaseManagementParameters parameters, CancellationToken cancellationToken)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var response = await CaseManagementManager.GetCases(agentId, parameters, cancellationToken);
            if (response == null)
            {
                Logger.LogError("Failed to retrieve cases for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("FilterOptions")]
    [ProducesResponseType(typeof(CaseManagementFilters), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetFilterOptions(CancellationToken cancellationToken)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var response = await CaseManagementManager.GetFilterOptions(agentId, cancellationToken);
            if (response == null)
            {
                Logger.LogError("Failed to retrieve filter options for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("ResendEmail")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ResendEmail(string envelopeId, CancellationToken cancellationToken)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var response = await CaseManagementManager.ResendEmail(envelopeId, cancellationToken);
            if (response == false)
            {
                Logger.LogError("Failed to resend email for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}