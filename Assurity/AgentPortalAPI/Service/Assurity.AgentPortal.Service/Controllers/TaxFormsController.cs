namespace Assurity.AgentPortal.Service.Controllers;

using System.Net;
using System.Net.Mime;
using Assurity.AgentPortal.Contracts.TaxForms;
using Assurity.AgentPortal.Managers.TaxForms;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Service.IOCConfig.Extensions;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("/API/[controller]")]
[ApiController]
[Authorize(AuthorizationPolicyConstants.TaxForms)]
public class TaxFormsController(
    ILogger<TaxFormsController> logger,
    ITaxFormsManager taxFormsManager,
    IConfigurationManager configurationManager) : BaseController(configurationManager, logger)
{
    private ITaxFormsManager TaxFormsManager { get; } = taxFormsManager;

    /// <summary>
    /// Get all agent forms for which a given agent has access.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("agents/forms", Name = nameof(GetTaxForms))]
    [ProducesResponseType(typeof(List<TaxForm>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTaxForms(
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
            var response = await TaxFormsManager.GetTaxForms(agentId, cancellationToken);
            if (response == null)
            {
                Logger.LogError("Failed to retrieve tax forms for agent {AgentId}", agentId);
                return ErrorResponse();
            }

            return Ok(response);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }

    /// <summary>
    /// Returns a tax form in PDF format.
    /// </summary>
    /// ## Useful Agent IDs
    /// - 2XM9
    /// - 29Y9
    /// - 06EB.
    /// ## Userful Form IDs
    /// - 6C113F8C5F202F9E5E2317ED75932650!F195DDF81E23D69E857E9D2DE921F847
    /// <param name="formId"/>
    /// <param name="cancellationToken"/>
    /// <returns></returns></remarks>
    [HttpGet("agents/forms/{formId}", Name = nameof(GetTaxForm))]
    [ProducesResponseType(typeof(TaxForm), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTaxForm(
        string formId,
        CancellationToken cancellationToken)
    {
        var agentId = HttpContext.GetAgentId();
        if (string.IsNullOrWhiteSpace(agentId))
        {
            Logger.LogWarning("Invalid Agent Id retrieved from access token.");
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(formId))
        {
            Logger.LogWarning("Form Id required.");

            var problemDetails = GetProblemDetails(
                HttpStatusCode.BadRequest,
                StatusCodes.Status400BadRequest,
                "Form Id required.");

            return BadRequest(problemDetails);
        }

        try
        {
            var response = await TaxFormsManager.GetTaxForm(agentId, formId, cancellationToken);
            if (response == null)
            {
                Logger.LogError("Failed to retrieve tax form {TaxFormId} for agent {AgentId}", formId, agentId);
                return ErrorResponse();
            }

            var mimeType = MediaTypeNames.Application.Pdf;

            return File(response.FileData, mimeType, response.FileName);
        }
        catch (Exception exception)
        {
            return HandleException(exception, Logger);
        }
    }
}
