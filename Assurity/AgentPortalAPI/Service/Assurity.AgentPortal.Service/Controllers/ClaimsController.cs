namespace Assurity.AgentPortal.Service.Controllers;

using System.Threading;
using Assurity.AgentPortal.Contracts.Claims;
using Assurity.AgentPortal.Managers.Claims;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.Claims)]
public class ClaimsController(
    ILogger<ClaimsController> logger,
    IConfigurationManager configurationManager,
    IClaimsManager claimsManager) : BaseController(configurationManager, logger)
{
    private IClaimsManager ClaimsManager { get; } = claimsManager;

    [HttpGet]
    [ProducesResponseType(typeof(Contracts.Claims.ClaimsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> GetClaims([FromQuery] ClaimsParameters parameters, CancellationToken cancellationToken)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        if (parameters == null || (
            string.IsNullOrEmpty(parameters.ClaimNumber)
            && string.IsNullOrEmpty(parameters.PolicyNumber)
            && string.IsNullOrEmpty(parameters.ClaimantFirstName)
            && string.IsNullOrEmpty(parameters.ClaimantLastName)))
        {
            return ErrorResponse("One of ClaimNumber, PolicyNumber, ClaimantFirstName or ClaimantLastName is Required.");
        }

        try
        {
            var claimsResponse = await ClaimsManager.GetClaims(agentId, parameters, cancellationToken);

            if (claimsResponse == null)
            {
                Logger.LogError("Failed to retrieve claims for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(claimsResponse);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}