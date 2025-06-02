namespace Assurity.AgentPortal.Service.Validation;

using Assurity.AgentPortal.Contracts.Send;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public interface IFileValidator
{
    bool IsMultipartContentType(string? contentType);

    Task<File> ProcessFormFile(
        IFormFile formFile,
        ModelStateDictionary modelState);
}