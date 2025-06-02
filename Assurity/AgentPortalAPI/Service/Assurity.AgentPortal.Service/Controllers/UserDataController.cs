namespace Assurity.AgentPortal.Service.Controllers;

using Assurity.AgentPortal.Contracts.AgentContracts;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.UserData;
using Assurity.AgentPortal.Contracts.UserData.Requests;
using Assurity.AgentPortal.Managers.AgentHierarchy;
using Assurity.AgentPortal.Managers.UserData;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize]
public class UserDataController(
    ILogger<UserDataController> logger,
    IConfigurationManager configurationManager,
    IAgentHierarchyManager agentManager,
    IUserDataManager userDataManager) : BaseController(configurationManager, logger)
{
    private IUserDataManager UserDataManager { get; } = userDataManager;

    private IAgentHierarchyManager AgentManager { get; } = agentManager;

    [HttpGet("BusinessTypes", Name = nameof(GetBusinessTypes))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetBusinessTypes(
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
            var businessTypes = await UserDataManager.GetBusinessTypesByAgentId(agentId, cancellationToken);
            if (businessTypes == null)
            {
                Logger.LogError("Failed to retrieve business types for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(businessTypes);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("ActiveContracts", Name = nameof(GetAgentContracts))]
    [ProducesResponseType(typeof(Contracts.AgentContracts.AgentContractsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAgentContracts(
        CancellationToken cancellationToken,
        [FromQuery] bool includeAssociatedAgentNumbers = false,
        [FromQuery] MarketCodeFilters marketCodeFilter = MarketCodeFilters.None,
        [FromQuery] string? agentStatusFilters = null)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var response = await AgentManager.GetAgentContracts(agentId, includeAssociatedAgentNumbers, marketCodeFilter, cancellationToken, agentStatusFilters);
            if (response == null)
            {
                Logger.LogError("Failed to retrieve agent contracts for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("RegionCodeInformation/{regionCode}", Name = nameof(GetRegionCodeInformation))]
    [ProducesResponseType(typeof(Contracts.AgentContracts.MarketCodeInformationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRegionCodeInformation(
        [FromRoute] string regionCode)
    {
        try
        {
            var response = await AgentManager.GetMarketCodeInformation(regionCode);
            if (response == null)
            {
                Logger.LogError("Failed to retrieve market code information for market code {MarketCode}", regionCode);
                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex, Logger);
        }
    }

    [HttpGet("VertaforeInformation", Name = nameof(GetVertaforeInformation))]
    [ProducesResponseType(typeof(VertaforeInformationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetVertaforeInformation(
        CancellationToken cancellationToken)
    {
        try
        {
            var agentId = HttpContext.GetAgentId();
            if (string.IsNullOrEmpty(agentId))
            {
                Logger.LogWarning("Invalid Agent Id retrieved from access token.");
                return ErrorResponse();
            }

            var username = HttpContext.GetAgentUsername();
            if (string.IsNullOrEmpty(username))
            {
                Logger.LogWarning("Invalid username retrieved from access token for agent {AgentId}.", agentId);
                return ErrorResponse();
            }

            var email = HttpContext.GetAgentEmail();
            if (string.IsNullOrEmpty(email))
            {
                Logger.LogWarning("Invalid email retrieved from access token for agent {AgentId}.", agentId);
                return ErrorResponse();
            }

            var response = await AgentManager.GetVertaforeInformation(
                agentId,
                username,
                email,
                cancellationToken);

            if (response == null)
            {
                Logger.LogError("Failed to retrieve vertafore information for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex, Logger);
        }
    }

    [HttpGet("ViewAsAgents", Name = nameof(GetViewAsAgents))]
    [ProducesResponseType(typeof(List<DropdownOption>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetViewAsAgents(
        CancellationToken cancellationToken,
        [FromQuery] bool includeAssociatedAgentNumbers = true,
        [FromQuery] MarketCodeFilters marketCodeFilter = MarketCodeFilters.None)
    {
        try
        {
            var agentId = HttpContext.GetAgentId();

            if (string.IsNullOrWhiteSpace(agentId))
            {
                Logger.LogWarning("Invalid Agent Id retrieved from access token.");
                return Unauthorized();
            }

            var response = await AgentManager.GetViewAsDropdownOptions(agentId, includeAssociatedAgentNumbers, marketCodeFilter, cancellationToken);
            if (response == null)
            {
                Logger.LogError("Failed to retrieve view as dropdown options for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex, Logger);
        }
    }

    [HttpGet("AgentAccess", Name = nameof(GetAgentAccess))]
    [ProducesResponseType(typeof(AccessLevel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAgentAccess(CancellationToken cancellationToken)
    {
        try
        {
            var agentId = HttpContext.GetAgentId();

            if (string.IsNullOrWhiteSpace(agentId))
            {
                Logger.LogWarning("Invalid Agent Id retrieved from access token.");
                return Unauthorized();
            }

            var response = await UserDataManager.GetAgentAccess(agentId, cancellationToken);
            if (response == null)
            {
                Logger.LogError("Failed to retrieve agent access for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            return HandleException(ex, Logger);
        }
    }

    [AllowAnonymous]
    [HttpPost("SendEmailNotifications", Name = nameof(SendEmailNotifications))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendEmailNotifications([FromBody] SendEmailNotificationsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.OriginalEmail) || string.IsNullOrEmpty(request.NewEmail))
            {
                return BadRequestResponse("Original or new email values are missing");
            }

            await UserDataManager.SendEmailNotifications(request.OriginalEmail, request.NewEmail);

            return Ok();
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("AdditionalAgentIds", Name = nameof(AdditionalAgentIdsResponse))]
    [ProducesResponseType(typeof(AdditionalAgentIdsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAdditionalAgentIds(CancellationToken cancellationToken)
    {
        try
        {
            var agentId = HttpContext.GetAgentId();

            if (string.IsNullOrWhiteSpace(agentId))
            {
                Logger.LogWarning("Invalid Agent Id retrieved from access token.");
                return Unauthorized();
            }

            var response = await UserDataManager.GetAdditionalAgentIds(agentId, cancellationToken);
            if (response == null)
            {
                Logger.LogError("Failed to retrieve additional agents for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            var additionalAgentIdsResponse = new AdditionalAgentIdsResponse
            {
                AgentIds = response
            };

            return Ok(additionalAgentIdsResponse);
        }
        catch (Exception ex)
        {
            return HandleException(ex, Logger);
        }
    }
}