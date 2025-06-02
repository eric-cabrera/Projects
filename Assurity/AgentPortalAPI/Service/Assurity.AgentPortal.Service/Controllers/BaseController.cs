namespace Assurity.AgentPortal.Service.Controllers;

using System;
using System.Net;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public abstract class BaseController(IConfigurationManager configuration, ILogger logger) : ControllerBase
{
    internal IConfigurationManager ConfigurationManager { get; } = configuration;

    internal ILogger Logger { get; } = logger;

    internal static ProblemDetails GetProblemDetails(HttpStatusCode httpStatusCode, int statusCode, string message)
    {
        return new ProblemDetails
        {
            Title = httpStatusCode.ToString(),
            Status = statusCode,
            Detail = message ?? string.Empty,
        };
    }

    /// <summary>
    /// Creates an Object result with generated Problem details based on the provieded HttpStatusCode and message.
    /// </summary>
    /// <param name="message">The message to display with the error.</param>
    /// <param name="httpStatusCode">The HttpStatusCode, defaulted to InternalServerError.</param>
    /// <param name="statusCode">The StatusCode, defaulted to Status500InternalServerError.</param>
    /// <returns>Returns an Object result with generated Problem details.</returns>
    internal static IActionResult ErrorResponse(
        string? message = null,
        HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError,
        int statusCode = StatusCodes.Status500InternalServerError)
    {
        var problemDetails = GetProblemDetails(
            httpStatusCode,
            statusCode,
            message ?? "An unexpected error occured.");

        return new ObjectResult(problemDetails);
    }

    internal static IActionResult ErrorResponse(
       ModelStateDictionary modelState,
       HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest,
       int statusCode = StatusCodes.Status400BadRequest)
    {
        var errorMessages = modelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        var message = errorMessages.Any()
            ? string.Join("; ", errorMessages)
            : "Invalid request parameters.";

        var problemDetails = GetProblemDetails(httpStatusCode, statusCode, message);

        return new ObjectResult(problemDetails);
    }

    /// <summary>
    /// Creates a BadRequest() with ProblemDetails of HttpStatusCode.BadRequest and StatusCodes.Status400BadRequest.
    /// </summary>
    /// <param name="message">The message displayed.</param>
    /// <returns>Returns a BadRequestObjectResult.</returns>
    internal IActionResult BadRequestResponse(string? message = null)
    {
        var problemDetails = GetProblemDetails(
            HttpStatusCode.BadRequest,
            StatusCodes.Status400BadRequest,
            message ?? "Missing required parameter(s).");

        return BadRequest(problemDetails);
    }

    /// <summary>
    /// Interprets a caught exception, whether it was a canceled operation or an unhandled exception and logs the issue.
    /// </summary>
    /// <param name="exception">The thrown exception.</param>
    /// <param name="logger">The logger used to log the issue.</param>
    /// <returns>Either a 409 StatusCode for a canceled operation or an ErrorResponse for an unhandled exception.</returns>
    internal IActionResult HandleException(Exception exception, ILogger logger)
    {
        switch (exception)
        {
            case OperationCanceledException:
                logger.LogInformation("Request was cancelled. {ExceptionMessage}", exception.Message);
                return StatusCode(StatusCodes.Status409Conflict);
            case UnauthorizedAccessException:
                logger.LogWarning("Access is denied. {ExceptionMessage}", exception.Message);
                return StatusCode(StatusCodes.Status403Forbidden, "Access is denied.");
            case TimeoutException:
                logger.LogError("A timeout occurred. {ExceptionMessage}", exception.Message);
                return ErrorResponse("An Unhandled Exception Occurred.", HttpStatusCode.InternalServerError, StatusCodes.Status500InternalServerError);
            default:
                logger.LogError(exception, "An Unhandled Exception Occurred.");
                return ErrorResponse("An Unhandled Exception Occurred.", HttpStatusCode.InternalServerError, StatusCodes.Status500InternalServerError);
        }
    }
}
