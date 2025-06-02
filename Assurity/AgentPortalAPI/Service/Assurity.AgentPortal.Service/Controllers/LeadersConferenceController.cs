namespace Assurity.AgentPortal.Service.Controllers;

using Assurity.AgentPortal.Contracts.LeadersConference;
using Assurity.AgentPortal.Managers.LeadersConference;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize]
public class LeadersConferenceController(
    ILogger<LeadersConferenceController> logger,
    IConfigurationManager configurationManager,
    ILeadersConferenceManager leadersConferenceManager) : BaseController(configurationManager, logger)
{
    private ILeadersConferenceManager LeadersConferenceManager { get; } = leadersConferenceManager;

    [HttpGet("QualificationStatus", Name = nameof(GetQualificationStatus))]
    [ProducesResponseType(typeof(QualificationStatusSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetQualificationStatus(
        [FromQuery] int year,
        [FromQuery] QualiferType qualiferType,
        CancellationToken cancellationToken)
    {
        var loggedInAgentNumber = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(loggedInAgentNumber))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var response = LeadersConferenceManager.GetQualificationStatus(loggedInAgentNumber, year, qualiferType, cancellationToken);
            if (response == null)
            {
                Logger.LogError("Failed to retrieve qualification status for agent {AgentId}", loggedInAgentNumber);
                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("LeadersQualifiers", Name = nameof(GetLeadersQualifiers))]
    [ProducesResponseType(typeof(QualificationStatusSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetLeadersQualifiers([FromQuery] int year, CancellationToken cancellationToken)
    {
        var loggedInAgentNumber = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(loggedInAgentNumber))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var response = LeadersConferenceManager.GetLeadersQualifiers(loggedInAgentNumber, year, cancellationToken);
            if (response == null)
            {
                Logger.LogError("Failed to retrieve leaders qualifiers for agent {AgentId}", loggedInAgentNumber);
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
