namespace Assurity.AgentPortal.Service.Models;

using Assurity.AgentPortal.Service.ModelBinders;
using Microsoft.AspNetCore.Mvc;

public class UploadRequest
{
    public List<IFormFile>? FilesToUpload { get; set; }

    [ModelBinder(typeof(FormDataJsonModelBinder))]
    public UploadData UploadData { get; set; }
}