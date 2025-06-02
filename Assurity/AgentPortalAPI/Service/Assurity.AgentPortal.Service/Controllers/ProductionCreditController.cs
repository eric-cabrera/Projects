namespace Assurity.AgentPortal.Service.Controllers;

using Assurity.AgentPortal.Contracts.Enums;
using Assurity.AgentPortal.Contracts.ProductionCredit.FileExport;
using Assurity.AgentPortal.Contracts.ProductionCredit.Request;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Individual;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.PolicyDetail;
using Assurity.AgentPortal.Contracts.ProductionCredit.Response.Worksite;
using Assurity.AgentPortal.Contracts.Shared;
using Assurity.AgentPortal.Managers.ProductionCredit;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.ProductionCredit)]
public class ProductionCreditController(
    ILogger<ProductionCreditController> logger,
    IConfigurationManager configurationManager,
    IProductionCreditManager productionCreditManager) : BaseController(configurationManager, logger)
{
    private IProductionCreditManager ProductionCreditManager { get; } = productionCreditManager;

    [HttpGet("Individual/Summary", Name = nameof(GetIndividualProductionCreditSummary))]
    [ProducesResponseType(typeof(IndividualProductionCreditSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetIndividualProductionCreditSummary(
        [FromQuery] ProductionCreditParameters parameters,
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
            var productionCreditData = await ProductionCreditManager.GetIndividualProductionCreditSummary(agentId, parameters, cancellationToken);
            if (productionCreditData == null)
            {
                Logger.LogError("Failed to retrieve individual production credit summary for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(productionCreditData);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Individual/PolicyDetails", Name = nameof(GetIndividualPolicyDetailsSummary))]
    [ProducesResponseType(typeof(ProductionCreditPolicyDetailsSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetIndividualPolicyDetailsSummary(
        [FromQuery] ProductionCreditPolicyDetailsParameters parameters,
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
            var productionCreditData = await ProductionCreditManager.GetIndividualPolicyDetailsSummary(agentId, parameters, cancellationToken);
            if (productionCreditData == null)
            {
                Logger.LogError("Failed to retrieve individual policy details for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(productionCreditData);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Worksite/Summary", Name = nameof(GetWorksiteProductionCreditSummary))]
    [ProducesResponseType(typeof(WorksiteProductionCreditSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetWorksiteProductionCreditSummary(
        [FromQuery] WorksiteProductionCreditParameters parameters,
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
            var productionCreditData = await ProductionCreditManager.GetWorksiteProductionCreditSummary(agentId, parameters, cancellationToken);
            if (productionCreditData == null)
            {
                Logger.LogError("Failed to retrieve worksite production credit summary for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(productionCreditData);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Worksite/PolicyDetails", Name = nameof(GetWorksitePolicyDetailsSummary))]
    [ProducesResponseType(typeof(ProductionCreditPolicyDetailsSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetWorksitePolicyDetailsSummary(
        [FromQuery] ProductionCreditPolicyDetailsParameters parameters,
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
            var productionCreditData = await ProductionCreditManager.GetWorksitePolicyDetailsSummary(agentId, parameters, cancellationToken);
            if (productionCreditData == null)
            {
                Logger.LogError("Failed to retrieve worksite policy details summary for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(productionCreditData);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Individual/PolicyDetails/Export", Name = nameof(ExportIndividualPolicyDetailsSummary))]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportIndividualPolicyDetailsSummary(
        [FromQuery] ProductionCreditPolicyDetailsParameters parameters,
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
            var summaryFile = await ProductionCreditManager.ExportIndividualPolicyDetailsSummary(agentId, parameters, cancellationToken);
            if (summaryFile == null || summaryFile.FileData == null)
            {
                Logger.LogError("Failed to retrieve individual policy details summary download for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return File(summaryFile.FileData, summaryFile.FileType, summaryFile.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Worksite/PolicyDetails/Export", Name = nameof(ExportWorksitePolicyDetailsSummary))]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ExportWorksitePolicyDetailsSummary(
        [FromQuery] ProductionCreditPolicyDetailsParameters parameters,
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
            var summaryFile = await ProductionCreditManager.ExportWorksitePolicyDetailsSummary(agentId, parameters, cancellationToken);
            if (summaryFile == null || summaryFile.FileData == null)
            {
                Logger.LogError("Failed to retrieve worksite policy details summary download for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return File(summaryFile.FileData, summaryFile.FileType, summaryFile.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Individual/ByGrouping/Export", Name = nameof(GetIndividualProductionCreditByGrouping))]
    [ProducesResponseType(typeof(FileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetIndividualProductionCreditByGrouping(
    [FromQuery] ProductionCreditParameters parameters,
    [FromQuery] GroupingType groupingType,
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
            var fileResponse = await ProductionCreditManager.GetIndividualProductionCreditByGrouping(agentId, parameters, groupingType, cancellationToken);
            if (fileResponse == null)
            {
                Logger.LogError("Failed to generate grouped production credit report for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return File(fileResponse.FileData, fileResponse.FileType, $"{fileResponse.FileName}.xlsx");
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    [HttpGet("Worksite/NestedTables/ByTaps", Name = nameof(GetWorksiteProductionCreditNestedTablesByTaps))]
    [ProducesResponseType(typeof(List<ProductionCreditGroupedExport>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetWorksiteProductionCreditNestedTablesByTaps(
    [FromQuery] ProductionCreditViewType tap,
    [FromQuery] WorksiteProductionCreditParameters parameters,
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
            var fileResponse = await ProductionCreditManager.GetWorksiteProductionCreditExcelByTaps(
                agentId,
                tap,
                parameters,
                cancellationToken);

            if (fileResponse == null)
            {
                Logger.LogError("Failed to generate grouped **worksite** production credit report for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return File(fileResponse.FileData, fileResponse.FileType, $"{fileResponse.FileName}.xlsx");
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}
