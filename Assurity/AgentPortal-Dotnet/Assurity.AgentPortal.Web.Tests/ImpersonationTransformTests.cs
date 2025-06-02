namespace Assurity.AgentPortal.Web.Tests;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using Assurity.AgentPortal.Contracts;
using Assurity.AgentPortal.Web.ClientUtilities;
using Assurity.Common.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

public class ImpersonationTransformTests
{
    [Fact]
    public async Task ApplyAsync_AddsImpersonationCookie()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Environment", "LOCAL" },
        };

        IConfiguration configuration = new ConfigurationBuilder()
                              .AddInMemoryCollection(inMemorySettings)
                              .Build();

        var configurationManager = new Utilities.ConfigurationManager(configuration, new AesEncryptor());

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/impersonation/impersonate";
        httpContext.Request.Method = "POST";

        var responseBody = new ImpersonationRecord
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "Test",
            Name = "Testerson",
            Email = "test@test.com",
            Agents = new List<AgentRecord>
            {
               new AgentRecord
               {
                   Name = "test",
                   AgentIds = new List<string>
                   {
                       "123"
                   }
               }
            },
            RegisteredAgentId = "123",
        };

        var proxyResponse = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(JsonSerializer.Serialize(responseBody)),
        };
        proxyResponse.StatusCode = System.Net.HttpStatusCode.OK;
        var mockLogger = new Mock<ILogger<ImpersonationTransform>>();
        var transformProvider = new ImpersonationTransform(configurationManager, mockLogger.Object);
        var transformContext = new TransformBuilderContext();

        transformProvider.Apply(transformContext);

        var transform = transformContext.ResponseTransforms.First();

        var responseContext = new ResponseTransformContext
        {
            HttpContext = httpContext,
            ProxyResponse = proxyResponse,
        };

        // Act
        await transform.ApplyAsync(responseContext);

        // Assert
        var cookies = httpContext.Response.Headers.SetCookie;
        var parsedCookieValue = ParseCookieValue(cookies.First());
        var deserializedResponse = JsonSerializer.Deserialize<AgentData>(parsedCookieValue);
        var expectedCookie = ConvertImpersonationToAgentData(responseBody);
        Assert.Equivalent(expectedCookie, deserializedResponse);
        Assert.Equal(StatusCodes.Status200OK, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task ApplyAsync_InvalidResponse_ShouldNotSetCookie()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Environment", "LOCAL" },
        };

        IConfiguration configuration = new ConfigurationBuilder()
                              .AddInMemoryCollection(inMemorySettings)
                              .Build();

        var configurationManager = new Utilities.ConfigurationManager(configuration, new AesEncryptor());

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/impersonation/impersonate";
        httpContext.Request.Method = "POST";
        httpContext.Response.Body = new MemoryStream();

        var proxyResponse = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent("Howdy"),
        };

        var mockLogger = new Mock<ILogger<ImpersonationTransform>>();
        var transformProvider = new ImpersonationTransform(configurationManager, mockLogger.Object);
        var transformContext = new TransformBuilderContext();

        transformProvider.Apply(transformContext);

        var transform = transformContext.ResponseTransforms.First();

        var responseContext = new ResponseTransformContext
        {
            HttpContext = httpContext,
            ProxyResponse = proxyResponse,
        };

        // Act
        await transform.ApplyAsync(responseContext);

        // Assert
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using StreamReader reader = new(httpContext.Response.Body);
        var body = await reader.ReadToEndAsync();

        Assert.Equal(0, httpContext.Response.Headers.SetCookie.Count);
        Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
        Assert.Equal("An error occured.", JsonNode.Parse(body)["Text"].ToString());
    }

    private static string ParseCookieValue(string cookieHeader)
    {
        var parts = cookieHeader.Split(";", StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts.Select(p => p.Trim()))
        {
            if (part.Contains('='))
            {
                var keyValue = part.Split('=', 2);

                return HttpUtility.UrlDecode(keyValue[1].Trim());
            }
        }

        return string.Empty;
    }

    private static AgentData ConvertImpersonationToAgentData(ImpersonationRecord impersonationRecord)
    {
        return new AgentData
        {
            Id = impersonationRecord.Id,
            AgentId = impersonationRecord.RegisteredAgentId ?? impersonationRecord.Agents.First().AgentIds.First(),
            Username = impersonationRecord.UserName,
            Name = impersonationRecord.Name,
            Email = impersonationRecord.Email,
            AdditionalAgents = impersonationRecord.Agents,
        };
    }
}
