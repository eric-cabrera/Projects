namespace Assurity.AgentPortal.Service.Controllers;

using System;
using Assurity.AgentPortal.Contracts.Subaccounts;
using Assurity.AgentPortal.Contracts.Subaccounts.Request;
using Assurity.AgentPortal.Managers.Subaccounts;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("/API/[controller]")]
[ApiController]
public class SubaccountsController(
    ISubaccountManager subaccountManager,
    ILogger<SubaccountsController> logger,
    IConfigurationManager configurationManager) : BaseController(configurationManager, logger)
{
    private ISubaccountManager SubaccountManager { get; } = subaccountManager;

    [HttpGet(Name = nameof(GetPendingSubaccounts))]
    [ProducesResponseType(typeof(SubaccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPendingSubaccounts(CancellationToken cancellationToken)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var subaccounts = await SubaccountManager.GetPendingSubaccounts(agentId, cancellationToken);
            if (subaccounts == null)
            {
                Logger.LogError("Failed to retrieve pending subaccounts for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(subaccounts);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [AllowAnonymous]
    [HttpPost("{id}", Name = nameof(ActivateSubaccount))]
    [ProducesResponseType(typeof(PendingSubaccountActivationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ActivateSubaccount([FromBody] ActivationRequest request, string id)
    {
        try
        {
            var result = await SubaccountManager.ActivateSubaccount(request.Email, Guid.Parse(id));
            if (result == null)
            {
                Logger.LogError("Failed to activate subaccount for email {Email}", request.Email);
                return ErrorResponse();
            }

            return Ok(result);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [AllowAnonymous]
    [HttpDelete("{id}", Name = nameof(DeleteSubaccountById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSubaccountById(string id)
    {
        try
        {
            await SubaccountManager.DeletePendingSubaccount(id);

            return Ok();
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpPut(Name = nameof(UpdateSubaccount))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSubaccount([FromBody] SubaccountRequest request)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            await SubaccountManager.UpdateSubaccount(agentId, request.Email, request.Roles);

            return Ok();
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpPost("Notify", Name = nameof(ResendActivationEmail))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ResendActivationEmail([FromBody] ActivationRequest request)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            await SubaccountManager.ResendActivationEmail(agentId, request.Email);

            return Ok();
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpPost("DoesExist", Name = nameof(DoesSubaccountExist))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DoesSubaccountExist([FromBody] SubaccountRequest request)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent data retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var result = await SubaccountManager.DoesSubaccountExist(agentId, request.Email);

            return Ok(result);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpPost(Name = nameof(CreateSubaccount))]
    [ProducesResponseType(typeof(PendingSubaccount), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSubaccount([FromBody] SubaccountRequest request)
    {
        try
        {
            var agentId = HttpContext.GetAgentId();
            var username = HttpContext.GetAgentUsername();

            if (string.IsNullOrWhiteSpace(agentId) || string.IsNullOrWhiteSpace(username))
            {
                Logger.LogWarning("Invalid Agent data retrieved from access token.");
                return Unauthorized();
            }

            var subaccount = await SubaccountManager.CreateNewSubaccount(agentId, username, request.Email, request.Roles);
            if (subaccount == null)
            {
                Logger.LogError("Failed to create new subaccount for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(subaccount);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}
