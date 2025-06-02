namespace Assurity.AgentPortal.Service.Controllers;

using System;
using System.ComponentModel.DataAnnotations;
using Assurity.AgentPortal.Contracts.PolicyDelivery.Request;
using Assurity.AgentPortal.Managers.PolicyDelivery;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.ExcludeSubAccounts)]
public class PolicyDeliveryController(
    ILogger<PolicyDeliveryController> logger,
    IConfigurationManager configurationManager,
    IPolicyDeliveryManager agentManager) : BaseController(configurationManager, logger)
{
    private IPolicyDeliveryManager PolicyDeliveryManager { get; } = agentManager;

    [HttpPost("UpdatePolicyDeliveryOptions", Name = nameof(UpdatePolicyDeliveryOptions))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePolicyDeliveryOptions(
    [FromQuery] DocumentConnectOptions documentConnectOptions,
    CancellationToken cancellationToken = default)
    {
        var loggedInAgentNumber = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(loggedInAgentNumber))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        if (documentConnectOptions.AgentLinkSelected && string.IsNullOrEmpty(documentConnectOptions.Email))
        {
            Logger.LogWarning("Email is required for agent {ViewAsAgentNumber}", documentConnectOptions.Email);
            return BadRequestResponse("Email is required.");
        }

        if (documentConnectOptions.AgentLinkSelected && !new EmailAddressAttribute().IsValid(documentConnectOptions.Email))
        {
            Logger.LogWarning("Invalid email format was given: {Email} | Agent {ViewAsAgentNumber}", documentConnectOptions.Email, documentConnectOptions.ViewAsAgentNumber);
            return BadRequestResponse("Invalid email format.");
        }

        if (string.IsNullOrEmpty(documentConnectOptions.ViewAsAgentNumber))
        {
            Logger.LogWarning("ViewAsAgentNumber is null or empty for agent {loggedInAgentNumber}", loggedInAgentNumber);
            return BadRequestResponse("ViewAsAgentNumber is required.");
        }

        try
        {
            var result = await PolicyDeliveryManager.UpdateAgentPolicyDeliveryOptions(
                loggedInAgentNumber,
                documentConnectOptions,
                cancellationToken);

            if (result.HasValue && result.Value)
            {
                return Ok(new { success = true, message = "Policy delivery options updated successfully." });
            }
            else
            {
                Logger.LogWarning("Failed to update policy delivery options for Agent ID: {AgentId}.", documentConnectOptions.ViewAsAgentNumber);
                return ErrorResponse();
            }
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("GetPolicyDeliveryOptions", Name = nameof(GetPolicyDeliveryOptions))]
    [ProducesResponseType(typeof(DocumentConnectOptions), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPolicyDeliveryOptions(
       [Required][FromQuery] string viewAsAgentNumber,
       [Required][FromQuery] string viewAsMarketCode,
       CancellationToken cancellationToken = default)
    {
        var loggedInAgentNumber = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(loggedInAgentNumber))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        if (string.IsNullOrEmpty(viewAsAgentNumber))
        {
            Logger.LogWarning("ViewAsAgentNumber is null or empty for agent {loggedInAgentNumber}", loggedInAgentNumber);
            return BadRequestResponse("ViewAsAgentNumber is required.");
        }

        if (string.IsNullOrEmpty(viewAsMarketCode))
        {
            Logger.LogWarning("ViewAsMarketCode is null or empty for agent {loggedInAgentNumber}", loggedInAgentNumber);
            return BadRequestResponse("ViewAsMarketCode is required.");
        }

        try
        {
            var documentConnectOptions = await PolicyDeliveryManager.GetPolicyDeliveryOptions(
               loggedInAgentNumber,
               viewAsAgentNumber,
               viewAsMarketCode,
               cancellationToken);

            if (documentConnectOptions == null)
            {
                Logger.LogError("Failed to retrieve policy delivery options for agent {AgentId}", loggedInAgentNumber);
                return ErrorResponse();
            }

            return Ok(documentConnectOptions);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}