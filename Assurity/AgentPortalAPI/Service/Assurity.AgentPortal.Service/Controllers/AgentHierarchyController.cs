namespace Assurity.AgentPortal.Service.Controllers;

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using Assurity.AgentPortal.Contracts.AgentContracts;
using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Managers.AgentHierarchy;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.Hierarchy)]
public class AgentHierarchyController(
    ILogger<AgentHierarchyController> logger,
    IConfigurationManager configurationManager,
    IAgentHierarchyManager agentHierarchyManager) : BaseController(configurationManager, logger)
{
    private IAgentHierarchyManager AgentHierarchyManager { get; } = agentHierarchyManager;

    [HttpGet("ViewAs", Name = nameof(GetViewAsFilterInformation))]
    [ProducesResponseType(typeof(List<DropdownOption>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetViewAsFilterInformation(CancellationToken cancellationToken)
    {
        var loggedInAgentNumber = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(loggedInAgentNumber))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var response = await AgentHierarchyManager.GetViewAsDropdownOptions(loggedInAgentNumber, true, MarketCodeFilters.AgentCenter, cancellationToken);
            if (response == null)
            {
                Logger.LogError("Failed to retrieve view as dropdown options for agent {AgentId}", loggedInAgentNumber);
                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("ActiveHierarchy", Name = nameof(GetActiveHierarchy))]
    [ProducesResponseType(typeof(ActiveHierarchyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActiveHierarchy(
        [Required] string agentNumber,
        [Required] string marketCode,
        [Required] string agentLevel,
        [Required] string companyCode,
        [FromQuery] ContractStatus? contractStatus,
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
            var hierarchyResponse = await AgentHierarchyManager.GetAgentHierarchy(
                loggedInAgentNumber,
                agentNumber,
                marketCode,
                agentLevel,
                companyCode,
                contractStatus,
                false, // includeAgentInformation
                false, // includePendingRequirements
                false, // filterAgentsWithoutPendingRequirements
                cancellationToken);

            if (hierarchyResponse == null)
            {
                Logger.LogError(
                  "Failed to retrieve active hierarchy for LoggedInAgentNumber: {LoggedInAgentNumber}, " +
                  "AgentNumber: {AgentNumber}, MarketCode: {MarketCode}, AgentLevel: {AgentLevel}, " +
                  "CompanyCode: {CompanyCode}, ContractStatus: {ContractStatus}",
                  loggedInAgentNumber,
                  agentNumber,
                  marketCode,
                  agentLevel,
                  companyCode,
                  contractStatus);

                return ErrorResponse();
            }

            return Ok(hierarchyResponse);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("HierarchyWithPendingRequirements", Name = nameof(GetHierarchyWithPendingRequirements))]
    [ProducesResponseType(typeof(PendingRequirementsHierarchyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetHierarchyWithPendingRequirements(
       [Required] string agentNumber,
       [Required] string marketCode,
       [Required] string agentLevel,
       [Required] string companyCode,
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
            var hierarchyResponse = await AgentHierarchyManager.GetPendingRequirementsHierarchy(
                loggedInAgentNumber,
                agentNumber,
                marketCode,
                agentLevel,
                companyCode,
                cancellationToken);

            if (hierarchyResponse == null)
            {
                Logger.LogError(
                  "Failed to retrieve hierarchy with pending requirements for LoggedInAgentNumber: {LoggedInAgentNumber}, " +
                  "AgentNumber: {AgentNumber}, MarketCode: {MarketCode}, AgentLevel: {AgentLevel}, " +
                  "CompanyCode: {CompanyCode}",
                  loggedInAgentNumber,
                  agentNumber,
                  marketCode,
                  agentLevel,
                  companyCode);

                return ErrorResponse();
            }

            return Ok(hierarchyResponse);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("ActiveAppointments", Name = nameof(GetActiveAppointments))]
    [ProducesResponseType(typeof(AgentAppointmentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActiveAppointments(
        [Required] string viewAsAgentNumber,
        [Required] string viewAsMarketCode,
        [Required] string viewAsAgentLevel,
        [Required] string viewAsCompanyCode,
        [Required] string downlineAgentNumber,
        [Required] string downlineMarketCode,
        [Required] string downlineAgentLevel,
        [Required] string downlineCompanyCode,
        [FromQuery] State state,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
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
            var response = await AgentHierarchyManager.GetActiveAppointments(
                loggedInAgentNumber,
                viewAsAgentNumber,
                viewAsMarketCode,
                viewAsAgentLevel,
                viewAsCompanyCode,
                downlineAgentNumber,
                downlineMarketCode,
                downlineAgentLevel,
                downlineCompanyCode,
                state,
                page,
                pageSize,
                cancellationToken);

            if (response == null)
            {
                Logger.LogError(
                 "No active appointments found for viewAsAgentNumber: {ViewAsAgentNumber}, " +
                 "viewAsMarketCode: {ViewAsMarketCode}, viewAsAgentLevel: {ViewAsAgentLevel}, " +
                 "viewAsCompanyCode: {ViewAsCompanyCode}, DownlineAgentNumber: {DownlineAgentNumber}, " +
                 "DownlineMarketCode: {DownlineMarketCode}, DownlineAgentLevel: {DownlineAgentLevel}, " +
                 "DownlineCompanyCode: {DownlineCompanyCode}",
                 viewAsAgentNumber,
                 viewAsMarketCode,
                 viewAsAgentLevel,
                 viewAsCompanyCode,
                 downlineAgentNumber,
                 downlineMarketCode,
                 downlineAgentLevel,
                 downlineCompanyCode);

                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("AgentInformation", Name = nameof(GetAgentContractInformation))]
    [ProducesResponseType(typeof(AgentContractInformation), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAgentContractInformation(
        [Required] string viewAsAgentNumber,
        [Required] string viewAsMarketCode,
        [Required] string viewAsAgentLevel,
        [Required] string viewAsCompanyCode,
        [Required] string downlineAgentNumber,
        [Required] string downlineMarketCode,
        [Required] string downlineAgentLevel,
        [Required] string downlineCompanyCode,
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
            var response = await AgentHierarchyManager.GetAgentInformation(
                loggedInAgentNumber,
                viewAsAgentNumber,
                viewAsMarketCode,
                viewAsAgentLevel,
                viewAsCompanyCode,
                downlineAgentNumber,
                downlineMarketCode,
                downlineAgentLevel,
                downlineCompanyCode,
                cancellationToken);

            if (response == null)
            {
                Logger.LogError(
                  "No contract information found for LoggedInAgentNumber: {LoggedInAgentNumber}, " +
                  "ViewAsAgentNumber: {ViewAsAgentNumber}, ViewAsMarketCode: {ViewAsMarketCode}, " +
                  "ViewAsAgentLevel: {ViewAsAgentLevel}, ViewAsCompanyCode: {ViewAsCompanyCode}, " +
                  "DownlineAgentNumber: {DownlineAgentNumber}, DownlineMarketCode: {DownlineMarketCode}, " +
                  "DownlineAgentLevel: {DownlineAgentLevel}, DownlineCompanyCode: {DownlineCompanyCode}",
                  loggedInAgentNumber,
                  viewAsAgentNumber,
                  viewAsMarketCode,
                  viewAsAgentLevel,
                  viewAsCompanyCode,
                  downlineAgentNumber,
                  downlineMarketCode,
                  downlineAgentLevel,
                  downlineCompanyCode);

                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Hierarchy/Export", Name = nameof(ExportActiveHierarchy))]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportActiveHierarchy(
      [Required] string agentNumber,
      [Required] string marketCode,
      [Required] string agentLevel,
      [Required] string companyCode,
      [FromQuery] ContractStatus contractStatus,
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
            var document = await AgentHierarchyManager.GetActiveHierarchyExport(
                loggedInAgentNumber,
                agentNumber,
                marketCode,
                agentLevel,
                companyCode,
                contractStatus,
                cancellationToken);

            if (document == null || document.FileData == null)
            {
                Logger.LogError(
                  "Failed to retrieve hierarchy export document for LoggedInAgentNumber: {LoggedInAgentNumber}, " +
                  "AgentNumber: {AgentNumber}, MarketCode: {MarketCode}, AgentLevel: {AgentLevel}, " +
                  "CompanyCode: {CompanyCode}, ContractStatus: {ContractStatus}",
                  loggedInAgentNumber,
                  agentNumber,
                  marketCode,
                  agentLevel,
                  companyCode,
                  contractStatus);

                return ErrorResponse();
            }

            return File(document.FileData, document.FileType, document.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("PendingRequirements/Export", Name = nameof(ExportPendingRequirements))]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportPendingRequirements(
      [Required] string agentNumber,
      [Required] string marketCode,
      [Required] string agentLevel,
      [Required] string companyCode,
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
            var document = await AgentHierarchyManager.GetActivePendingRequirementsExport(
                loggedInAgentNumber,
                agentNumber,
                marketCode,
                agentLevel,
                companyCode,
                cancellationToken);

            if (document == null || document.FileData == null)
            {
                Logger.LogError(
                  "Failed to retrieve pending requirements export document for Agent ID: {LoggedInAgentNumber}, " +
                  "AgentNumber: {AgentNumber}, MarketCode: {MarketCode}, AgentLevel: {AgentLevel}, CompanyCode: {CompanyCode}",
                  loggedInAgentNumber,
                  agentNumber,
                  marketCode,
                  agentLevel,
                  companyCode);

                return ErrorResponse();
            }

            return File(document.FileData, document.FileType, document.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Appointments/Export", Name = nameof(ExportAppointments))]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportAppointments(
        [Required] string viewAsAgentNumber,
        [Required] string viewAsMarketCode,
        [Required] string viewAsAgentLevel,
        [Required] string viewAsCompanyCode,
        [Required] string downlineAgentNumber,
        [Required] string downlineMarketCode,
        [Required] string downlineAgentLevel,
        [Required] string downlineCompanyCode,
        Contracts.Enums.State? state,
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
            var document = await AgentHierarchyManager.GetAppointmentsExport(
                loggedInAgentNumber,
                viewAsAgentNumber,
                viewAsMarketCode,
                viewAsAgentLevel,
                viewAsCompanyCode,
                downlineAgentNumber,
                downlineMarketCode,
                downlineAgentLevel,
                downlineCompanyCode,
                state,
                cancellationToken);

            if (document == null || document.FileData == null)
            {
                Logger.LogError(
                   "Failed to retrieve appointments export document for viewAsAgentNumber: {ViewAsAgentNumber}, " +
                   "viewAsMarketCode: {ViewAsMarketCode}, viewAsAgentLevel: {ViewAsAgentLevel}, " +
                   "viewAsCompanyCode: {ViewAsCompanyCode}, DownlineAgentNumber: {DownlineAgentNumber}, " +
                   "DownlineMarketCode: {DownlineMarketCode}, DownlineAgentLevel: {DownlineAgentLevel}, " +
                   "DownlineCompanyCode: {DownlineCompanyCode}",
                   viewAsAgentNumber,
                   viewAsMarketCode,
                   viewAsAgentLevel,
                   viewAsCompanyCode,
                   downlineAgentNumber,
                   downlineMarketCode,
                   downlineAgentLevel,
                   downlineCompanyCode);

                return ErrorResponse();
            }

            return File(document.FileData, document.FileType, document.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("DoesAgentHaveDownline", Name = nameof(DoesAgentHaveDownline))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DoesAgentHaveDownline(
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
            var result = await AgentHierarchyManager.DoesAgentHaveHierarchyDownline(
                loggedInAgentNumber,
                cancellationToken);

            return Ok(result);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}