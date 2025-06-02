namespace Assurity.AgentPortal.Service.Controllers;

using System;
using Assurity.AgentPortal.Contracts.Impersonation;
using Assurity.AgentPortal.Managers.Impersonation;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.HomeOfficePolicy)]
public class ImpersonationController(
    ILogger<ImpersonationController> logger,
    IConfigurationManager configurationManager,
    IImpersonationManager impersonationManager) : BaseController(configurationManager, logger)
{
    private IImpersonationManager ImpersonationManager { get; } = impersonationManager;

    [HttpGet("Search")]
    [ProducesResponseType(typeof(ImpersonationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Search(string term, CancellationToken cancellationToken)
    {
        try
        {
            var results = await ImpersonationManager.SearchAgents(term, cancellationToken);
            var response = new ImpersonationResponse { Records = results };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex, Logger);
        }
    }

    [HttpPost("Impersonate")]
    [ProducesResponseType(typeof(ImpersonationRecord), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ForbidResult), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Impersonate([FromBody] ImpersonationRecord impersonationRecord)
    {
        var usernameClaim = User.Claims.Where(x => x.Type == "preferred_username").FirstOrDefault();
        var userIdClaim = User.Claims.Where(x => x.Type == "sub").FirstOrDefault();

        if (usernameClaim is null || userIdClaim is null)
        {
            Logger.LogWarning("Invalid UserName or User Id retrieved from Claims.");
            return Unauthorized();
        }

        if (impersonationRecord.Id is null)
        {
            Logger.LogWarning("Invalid Impersonation Id for User Name {UserName} and User Id {UserId}.", usernameClaim, userIdClaim);
            return BadRequestResponse("Invalid Impersonation Id.");
        }

        try
        {
            var record = await ImpersonationManager.ImpersonateAgent(userIdClaim.Value, usernameClaim.Value, impersonationRecord.Id);
            if (record == null)
            {
                Logger.LogError("Failed to retrieve impersonate agent for agent {AgentId}", userIdClaim.Value);
                return ErrorResponse();
            }

            return Ok(record);
        }
        catch (Exception ex)
        {
            return HandleException(ex, Logger);
        }
    }

    [HttpGet("Recent")]
    [ProducesResponseType(typeof(ImpersonationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecentImpersonations(CancellationToken cancellationToken)
    {
        var userIdClaim = User.Claims.Where(x => x.Type == "sub").FirstOrDefault();

        if (userIdClaim is null)
        {
            Logger.LogWarning("Invalid UserName or User Id retrieved from Claims.");
            return Unauthorized();
        }

        try
        {
            var results = await ImpersonationManager.GetRecentImpersonations(userIdClaim.Value, cancellationToken);
            var response = new ImpersonationResponse { Records = results };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex, Logger);
        }
    }
}
