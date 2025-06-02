namespace Assurity.AgentPortal.Web.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Assurity.AgentPortal.Contracts;
using Assurity.AgentPortal.Utilities;
using Assurity.AgentPortal.Web.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class AgentDataMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_PingOneIssuer_ShouldSetHttpContextItems()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<AgentDataMiddleware>>();
        var username = "unAAXB";
        var agentId = "AAXB";
        var firstName = "John";
        var lastName = "Smith";
        var email = "test@email.com";
        var claims = new List<Claim>
        {
                new Claim("preferred_username", username, string.Empty, "PingOne"),
                new Claim("AgentID", agentId, string.Empty, "PingOne"),
                new Claim("role", "PendingActiveTerminated", string.Empty, "PingOne"),
                new Claim("given_name", firstName, string.Empty, "PingOne"),
                new Claim("family_name", lastName, string.Empty, "PingOne"),
                new Claim("email", email, string.Empty, "PingOne")
        };

        var mockIdentity = new Mock<ClaimsIdentity>();
        mockIdentity.Setup(identity => identity.Claims).Returns(claims);
        mockIdentity.Setup(identity => identity.IsAuthenticated).Returns(true);

        var user = new ClaimsPrincipal(mockIdentity.Object);

        var mockConfigurationManager = new Mock<IConfigurationManager>();
        mockConfigurationManager.Setup(config => config.PingOneAuthority).Returns("PingOne");

        var mockAccessToken = JwtTokenHelper.GenerateJwtToken(claims, "https://api.pingone.com");
        var mockServiceProvider = JwtTokenHelper.GetMockServiceProviderWithAccessToken(user, mockAccessToken);
        mockServiceProvider.Setup(x => x.GetService(typeof(IConfigurationManager))).Returns(mockConfigurationManager.Object);

        var httpContext = new DefaultHttpContext
        {
            User = user,
            RequestServices = mockServiceProvider.Object
        };

        httpContext.Response.Body = new MemoryStream();

        var agentDataMiddleware = new AgentDataMiddleware(new Mock<RequestDelegate>().Object, mockConfigurationManager.Object, mockLogger.Object);

        // Act
        await agentDataMiddleware.InvokeAsync(httpContext);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

        // Assert
        Assert.False((bool)httpContext.Items["IsHomeOfficeUser"]);
        Assert.True((bool)httpContext.Items["IsPingUser"]);
        Assert.Equal(httpContext.Items["Username"], username);
        Assert.Equal(httpContext.Items["AgentId"], agentId);
        Assert.Equal(httpContext.Items["Email"], email);
        Assert.Equal(httpContext.Items["FirstName"], firstName);
        Assert.Equal(httpContext.Items["LastName"], lastName);
    }

    [Fact]
    public async Task InvokeAsync_AzureAdIssuer_ShouldSetHttpContextItems()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<AgentDataMiddleware>>();

        var agentData = new AgentData
        {
            Username = "unAAXB",
            AgentId = "AAXB",
            Name = "Cool Agent",
            Email = "agent@assurity.com",
        };

        var username = "abc1234@assurity.com";
        var firstName = "a";
        var lastName = "b";
        var email = "abcdef@assurity.com";

        var claims = new List<Claim>
        {
                new Claim("preferred_username", username, string.Empty, "AzureAd"),
                new Claim("given_name", firstName, string.Empty, "AzureAd"),
                new Claim("family_name", lastName, string.Empty, "AzureAd"),
                new Claim("email", email, string.Empty, "AzureAd")
        };

        var mockIdentity = new Mock<ClaimsIdentity>();
        mockIdentity.Setup(identity => identity.Claims).Returns(claims);
        mockIdentity.Setup(identity => identity.IsAuthenticated).Returns(true);

        var user = new ClaimsPrincipal(mockIdentity.Object);

        var mockConfigurationManager = new Mock<IConfigurationManager>();
        mockConfigurationManager.Setup(config => config.AzureAdAuthority).Returns("AzureAd");
        mockConfigurationManager.Setup(config => config.Decrypt(It.IsAny<string>(), false)).Returns(JsonSerializer.Serialize(agentData));

        var mockAccessToken = JwtTokenHelper.GenerateJwtToken(claims, "https://api.pingone.com");
        var mockServiceProvider = JwtTokenHelper.GetMockServiceProviderWithAccessToken(user, mockAccessToken);
        mockServiceProvider.Setup(x => x.GetService(typeof(IConfigurationManager))).Returns(mockConfigurationManager.Object);

        var requestCookie = new Cookie
        {
            Secure = true,
            HttpOnly = true,
            Domain = ".assurity.com",
            Name = "__Host-ac-data",
            Value = JsonSerializer.Serialize(agentData),
        };

        var httpContext = new DefaultHttpContext
        {
            User = user,
            RequestServices = mockServiceProvider.Object,
        };

        var mockCookieCollection = new Mock<IRequestCookieCollection>();
        mockCookieCollection.Setup(c => c.TryGetValue(requestCookie.Name, out It.Ref<string>.IsAny))
            .Returns((string key, out string value) =>
            {
                value = requestCookie.Value;
                return requestCookie.Name.Equals(key);
            });

        httpContext.Request.Cookies = mockCookieCollection.Object;

        var agentDataMiddleware = new AgentDataMiddleware(new Mock<RequestDelegate>().Object, mockConfigurationManager.Object, mockLogger.Object);

        // Act
        await agentDataMiddleware.InvokeAsync(httpContext);

        // Assert
        Assert.True((bool)httpContext.Items["IsHomeOfficeUser"]);
        Assert.False((bool)httpContext.Items["IsPingUser"]);
        Assert.Equal(username, httpContext.Items["Username"]);
        Assert.Equal(agentData.AgentId, httpContext.Items["AgentId"]);
        Assert.Equal(email, httpContext.Items["Email"]);
        Assert.Equal(firstName, httpContext.Items["FirstName"]);
        Assert.Equal(lastName, httpContext.Items["LastName"]);
    }
}
