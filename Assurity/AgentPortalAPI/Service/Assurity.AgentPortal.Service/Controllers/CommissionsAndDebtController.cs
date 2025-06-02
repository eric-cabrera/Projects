namespace Assurity.AgentPortal.Service.Controllers;

using System;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Assurity.AgentPortal.Contracts.CommissionsDebt;
using Assurity.AgentPortal.Contracts.CommissionsDebt.Request;
using Assurity.AgentPortal.Managers.CommissionsAndDebt;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.CommissionsDebt)]
public class CommissionsAndDebtController(
    ILogger<CommissionsAndDebtController> logger,
    ICommissionAndDebtManager commissionsAndDebtManager,
    IConfigurationManager configurationManager) : BaseController(configurationManager, logger)
{
    private ICommissionAndDebtManager CommissionsAndDebtManager { get; } = commissionsAndDebtManager;

    [HttpGet("Commissions/PolicyDetails", Name = nameof(GetPolicyDetails))]
    [ProducesResponseType(typeof(CommissionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPolicyDetails(
        [FromQuery] PolicyDetailsParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var commissionData = await CommissionsAndDebtManager.GetCommissionAndSummaryData(agentId, parameters, cancellationToken);
            if (commissionData == null)
            {
                Logger.LogError("Failed to retrieve commission and summary data for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(commissionData);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Commissions/PolicyDetails/Export", Name = nameof(ExportPolicyDetails))]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportPolicyDetails(
    [FromQuery] PolicyDetailsParameters parameters,
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
            var file = await CommissionsAndDebtManager.GetPolicyDetailsExcel(
                agentId,
                parameters,
                cancellationToken);

            if (file == null || file.FileData == null)
            {
                Logger.LogError("Failed to retrieve policy details excel download for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return File(file.FileData, file.FileType, file.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Commissions/WritingAgentDetails", Name = nameof(GetWritingAgents))]
    [ProducesResponseType(typeof(WritingAgentDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetWritingAgents(
        [FromQuery] WritingAgentParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var commissionData = await CommissionsAndDebtManager.GetCommissionDataByWritingAgent(agentId, parameters, cancellationToken);
            if (commissionData == null)
            {
                Logger.LogError("Failed to retrieve commission data by writing agent for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(commissionData);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Commissions/WritingAgentDetails/Export", Name = nameof(ExportWritingAgentDetails))]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportWritingAgentDetails(
        [FromQuery] WritingAgentParameters parameters,
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
            var file = await CommissionsAndDebtManager.GetWritingAgentDetailsExcel(
                agentId,
                parameters,
                cancellationToken);

            if (file == null || file.FileData == null)
            {
                Logger.LogError("Failed to retrieve writing agent details excel download for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return File(file.FileData, file.FileType, file.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Commissions/AgentStatement", Name = nameof(GetAgentStatement))]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAgentStatement(
    [FromQuery] AgentStatementRequest agentStatementRequest,
    CancellationToken cancellationToken)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(agentStatementRequest.AgentId))
        {
            return BadRequestResponse("Agent ID is required.");
        }

        if (agentStatementRequest.CycleDate == null)
        {
            return BadRequestResponse("Cycle date is required.");
        }

        if (agentStatementRequest.AgentStatementType == null)
        {
            return BadRequestResponse("Agent statement type is required.");
        }

        try
        {
            var sessionId = HttpContext.User.FindFirstValue("sid");
            if (string.IsNullOrEmpty(sessionId))
            {
                Logger.LogError("Failed to retrieve session id for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            var agentStatement = await CommissionsAndDebtManager.GetAgentStatement(
                agentId,
                sessionId,
                agentStatementRequest.AgentId,
                agentStatementRequest.CycleDate.Value,
                agentStatementRequest.AgentStatementType.Value,
                cancellationToken);

            if (agentStatement == null)
            {
                Logger.LogError("Failed to retrieve agent statement for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return File(agentStatement, MediaTypeNames.Application.Pdf, "report.pdf");
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Commissions/AgentStatementOptions", Name = nameof(GetAgentStatementOptions))]
    [ProducesResponseType(typeof(AgentStatementOptions), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAgentStatementOptions(CancellationToken cancellationToken)
    {
        var agentId = HttpContext.GetAgentId();

        if (string.IsNullOrEmpty(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        try
        {
            var cycleDates = await CommissionsAndDebtManager.GetAgentStatementOptions(agentId, cancellationToken);
            if (cycleDates == null)
            {
                Logger.LogError("Failed to retrieve agent statement options for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(cycleDates);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Debt/UnsecuredAdvances", Name = nameof(GetUnsecuredAdvances))]
    [ProducesResponseType(typeof(DebtResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUnsecuredAdvances(
        [FromQuery] UnsecuredAdvanceParameters parameters,
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
            var debtResponse = await CommissionsAndDebtManager.GetUnsecuredAdvances(agentId, parameters, cancellationToken);
            if (debtResponse == null)
            {
                Logger.LogError("Failed to retrieve unsecured advances for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(debtResponse);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Debt/UnsecuredAdvances/Export", Name = nameof(ExportUnsecuredAdvances))]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportUnsecuredAdvances(
        [FromQuery] UnsecuredAdvanceParameters parameters,
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
            var file = await CommissionsAndDebtManager.GetUnsecuredAdvancesExcel(
                agentId,
                parameters,
                cancellationToken);

            if (file == null || file.FileData == null)
            {
                Logger.LogError("Failed to retrieve unsecured advances excel download for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return File(file.FileData, file.FileType, file.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Debt/SecuredAdvances", Name = nameof(GetSecuredAdvances))]
    [ProducesResponseType(typeof(DebtResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSecuredAdvances(
        [FromQuery] SecuredAdvanceParameters parameters,
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
            var debtResponse = await CommissionsAndDebtManager.GetSecuredAdvances(agentId, parameters, cancellationToken);
            if (debtResponse == null)
            {
                Logger.LogError("Failed to retrieve secured advances for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(debtResponse);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Debt/SecuredAdvances/Export", Name = nameof(ExportSecuredAdvances))]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportSecuredAdvances(
    [FromQuery] SecuredAdvanceParameters parameters,
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
            var file = await CommissionsAndDebtManager.GetSecuredAdvancesExcel(
                agentId,
                parameters,
                cancellationToken);

            if (file == null || file.FileData == null)
            {
                Logger.LogError("Failed to retrieve secured advances excel download for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return File(file.FileData, file.FileType, file.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}
