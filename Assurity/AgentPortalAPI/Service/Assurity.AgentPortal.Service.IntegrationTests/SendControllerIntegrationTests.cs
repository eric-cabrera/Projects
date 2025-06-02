namespace Assurity.AgentPortal.Service.IntegrationTests;

using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using Assurity.AgentPortal.Service.Models;
using Xunit;

[ExcludeFromCodeCoverage]
public class SendControllerIntegrationTests
{
    [Fact(Skip = "Integration")]
    public async Task TakeAction_Test()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new("https://localhost:7024/")
        };

        await using var jpgStream = File.OpenRead(@"C:\Users\sh2800\Desktop\TestImages\Dax.jpg");
        await using var pdfStream = File.OpenRead(@"C:\Users\sh2800\Desktop\TestImages\15-274-05051.pdf");

        var uploadData = new UploadData
        {
            Message = "Some sort of custom message from the user.",
            PolicyNumber = "4180075134"
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/send/takeAction");

        using var content = new MultipartFormDataContent
        {
            { new StreamContent(jpgStream), "FilesToUpload", "Dax.jpg" },
            { new StreamContent(pdfStream), "FilesToUpload", "15-274-05051.pdf" },
            { new StringContent(JsonSerializer.Serialize(uploadData), Encoding.UTF8, "application/json"), "UploadData" }
        };

        request.Content = content;

        await httpClient.SendAsync(request);
    }
}