namespace Assurity.AgentPortal.Web.UnitTests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Web.ClientUtilities;
using Assurity.AgentPortal.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class FeatureFlagMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_HappyPath_ShouldSucceed()
    {
        // Arrange
        var helperMock = new Mock<IFeatureFlagMiddlewareHelper>();
        helperMock.Setup(x => x.CheckIsWhiteListed(It.IsAny<PathString>())).Returns(false);
        helperMock.Setup(x => x.CheckFeatureIsInPathAndEnabled(It.IsAny<PathString>())).ReturnsAsync(true);
        var featureFlagMiddleware = new FeatureFlagMiddleware(new Mock<RequestDelegate>().Object, helperMock.Object);

        var hostBuilder = new HostBuilder()
        .ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton(helperMock.Object);
            });
            webHostBuilder.Configure(applicationBuilder =>
            {
                applicationBuilder.UseFeatureFlagMiddleware();
                applicationBuilder.Run(async context =>
                {
                    await context.Response.WriteAsync("Hello world");
                });
            });
            webHostBuilder.UseTestServer();
        });

        var testHost = await hostBuilder.StartAsync();
        var client = testHost.GetTestClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        Assert.True(response.StatusCode == System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public void InvokeAsync_WhiteListedPath_ShouldSucceed()
    {
        // Arrange
        var helperMock = new Mock<IFeatureFlagMiddlewareHelper>();
        helperMock.Setup(x => x.CheckIsWhiteListed(It.IsAny<PathString>())).Returns(true);
        helperMock.Setup(x => x.CheckFeatureIsInPathAndEnabled(It.IsAny<PathString>())).ReturnsAsync(false);

        var featureFlagMiddleware = new FeatureFlagMiddleware(new Mock<RequestDelegate>().Object, helperMock.Object);
        var httpContextMock = new Mock<HttpContext>();
        var requestMock = new Mock<HttpRequest>();
        var headers = new Mock<IHeaderDictionary>();

        requestMock.Setup(x => x.Path).Returns("/API/info");
        httpContextMock.Setup(x => x.Request).Returns(requestMock.Object);

        // Act
        var result = featureFlagMiddleware.InvokeAsync(httpContextMock.Object);

        // Assert
        Assert.True(result.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task InvokeAsync_DisabledFeature_404()
    {
        // Arrange
        var helperMock = new Mock<IFeatureFlagMiddlewareHelper>();
        helperMock.Setup(x => x.CheckIsWhiteListed(It.IsAny<PathString>())).Returns(false);
        helperMock.Setup(x => x.CheckFeatureIsInPathAndEnabled(It.IsAny<PathString>())).ReturnsAsync(false);
        var featureFlagMiddleware = new FeatureFlagMiddleware(new Mock<RequestDelegate>().Object, helperMock.Object);

        var hostBuilder = new HostBuilder()
        .ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton(helperMock.Object);
            });
            webHostBuilder.Configure(applicationBuilder =>
            {
                applicationBuilder.UseFeatureFlagMiddleware();
                applicationBuilder.Run(async context =>
                {
                    await context.Response.WriteAsync("Hello world");
                });
            });
            webHostBuilder.UseTestServer();
        });

        var testHost = await hostBuilder.StartAsync();
        var client = testHost.GetTestClient();

        // Act
        var response = await client.GetAsync("/");

        Assert.True(response.StatusCode == System.Net.HttpStatusCode.NotFound);
    }
}