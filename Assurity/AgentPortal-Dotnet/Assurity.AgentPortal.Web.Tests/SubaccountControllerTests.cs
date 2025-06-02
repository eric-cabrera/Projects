namespace Assurity.AgentPortal.Web.Tests;

using System.Security.Claims;
using Assurity.AgentPortal.Contracts.SubaccountUsers;
using Assurity.AgentPortal.Managers;
using Assurity.AgentPortal.Utilities;
using Assurity.AgentPortal.Web.Controllers;
using Assurity.AgentPortal.Web.Controllers.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Moq;
using Xunit;

public class SubaccountControllerTests
{
    [Fact]
    public async Task GetSubAccounts_Success()
    {
        // Arrange
        var userId = "56fbef87-1630-47b2-abcd-test134";
        var fakeAccessToken = "fakeAccessToken";
        var claims = new List<Claim>
        {
                new Claim("preferred_username", "unAAXB", string.Empty, "PingOne"),
                new Claim("AgentID", "AAXB", string.Empty, "AzureAd"),
                new Claim("role", "PendingActiveTerminated", string.Empty, "PingOne"),
                new Claim("given_name", "John", string.Empty, "PingOne"),
                new Claim("family_name", "Smith", string.Empty, "PingOne"),
                new Claim("email", "test@email.com", string.Empty, "PingOne")
        };

        var userBusinessTypes = new List<string> { "Individual" };
        var mockAccessToken = JwtTokenHelper.GenerateJwtToken(claims, "https://api.pingone.com");

        var mockIdentity = new Mock<ClaimsIdentity>();
        mockIdentity.Setup(identity => identity.Claims).Returns(claims);
        mockIdentity.Setup(identity => identity.IsAuthenticated).Returns(true);

        var user = new ClaimsPrincipal(mockIdentity.Object);

        var mockServiceProvider = JwtTokenHelper.GetMockServiceProviderWithAccessToken(user, mockAccessToken);

        var configManager = new Mock<IConfigurationManager>();
        configManager.Setup(config => config.PingOneAuthority).Returns("PingOne");

        var mockUserManager = new Mock<ISubaccountManager>();
        mockUserManager.Setup(manager => manager.GetSubaccountsAsync("AAXB", fakeAccessToken, CancellationToken.None, false)).ReturnsAsync(new List<GetUsersResponse>());

        var mockFeatureManager = new Mock<IFeatureManager>();
        mockFeatureManager.Setup(manager => manager.IsEnabledAsync("productioncredit"))
            .ReturnsAsync(true);

        var mockLogger = new Mock<ILogger<SubaccountController>>();

        var userController = new SubaccountController(mockUserManager.Object, configManager.Object, mockLogger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user,
                    RequestServices = mockServiceProvider.Object
                },
            }
        };

        // Act
        var result = await userController.GetSubaccounts(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateSubAccount_Success()
    {
        // Arrange
        var parameters = new UpdateSubAccountRequestParameters();

        var userManager = new Mock<ISubaccountManager>();
        var mockConfigManager = new Mock<IConfigurationManager>();
        var userController = new SubaccountController(userManager.Object, mockConfigManager.Object, null);

        // Act
        var result = await userController.UpdateSubaccountAsync(parameters, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteSubAccount_Success()
    {
        // Arrange
        var userId = "56fbef87-1630-47b2-abcd-test134";
        var parentAgentId = "124Test";
        var claims = new List<Claim>
        {
                new Claim("preferred_username", "unAAXB", string.Empty, "PingOne"),
                new Claim("AgentID", "AAXB", string.Empty, "AzureAd"),
                new Claim("role", "PendingActiveTerminated", string.Empty, "PingOne"),
                new Claim("given_name", "John", string.Empty, "PingOne"),
                new Claim("family_name", "Smith", string.Empty, "PingOne"),
                new Claim("email", "test@email.com", string.Empty, "PingOne")
        };

        var userBusinessTypes = new List<string> { "Individual" };
        var mockAccessToken = JwtTokenHelper.GenerateJwtToken(claims, "https://api.pingone.com");

        var mockIdentity = new Mock<ClaimsIdentity>();
        mockIdentity.Setup(identity => identity.Claims).Returns(claims);
        mockIdentity.Setup(identity => identity.IsAuthenticated).Returns(true);

        var user = new ClaimsPrincipal(mockIdentity.Object);

        var mockServiceProvider = JwtTokenHelper.GetMockServiceProviderWithAccessToken(user, mockAccessToken);

        var configManager = new Mock<IConfigurationManager>();
        configManager.Setup(config => config.PingOneAuthority).Returns("PingOne");

        var mockUserManager = new Mock<ISubaccountManager>();
        mockUserManager.Setup(manager =>
            manager.DeleteSubaccountAsync(userId, parentAgentId, true, "12312345123", CancellationToken.None))
            .ReturnsAsync(true);

        var mockFeatureManager = new Mock<IFeatureManager>();
        mockFeatureManager.Setup(manager => manager.IsEnabledAsync("productioncredit"))
            .ReturnsAsync(true);

        var mockLogger = new Mock<ILogger<SubaccountController>>();

        var userController = new SubaccountController(mockUserManager.Object, configManager.Object, mockLogger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user,
                    RequestServices = mockServiceProvider.Object
                },
            }
        };

        // Act
        var result = await userController.DeleteSubaccountAsync(userId, true, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task CreatePendingSubAccount_Success()
    {
        // Arrange
        var parameters = new CreateSubAccountRequest();

        var userManager = new Mock<ISubaccountManager>();
        var mockConfigManager = new Mock<IConfigurationManager>();
        var userController = new SubaccountController(userManager.Object, mockConfigManager.Object, null);

        // Act
        var result = await userController.CreatePendingSubaccountAsync(parameters, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ActivateSubAccount_Success()
    {
        // Arrange
        var parameters = new ActivateSubAccountRequest();

        var userManager = new Mock<ISubaccountManager>();
        var mockConfigManager = new Mock<IConfigurationManager>();
        var userController = new SubaccountController(userManager.Object, mockConfigManager.Object, null);

        // Act
        var result = await userController.ActivateSubaccountAsync(parameters, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ResendSubaccountActivationLink_Success()
    {
        // Arrange
        var emailParameter = "email@email.com";

        var userManager = new Mock<ISubaccountManager>();
        var mockConfigManager = new Mock<IConfigurationManager>();

        var claims = new List<Claim>
        {
                new Claim("preferred_username", "unAAXB", string.Empty, "PingOne"),
                new Claim("AgentID", "AAXB", string.Empty, "AzureAd"),
                new Claim("role", "PendingActiveTerminated", string.Empty, "PingOne"),
                new Claim("given_name", "John", string.Empty, "PingOne"),
                new Claim("family_name", "Smith", string.Empty, "PingOne"),
                new Claim("email", "test@email.com", string.Empty, "PingOne")
        };

        var userBusinessTypes = new List<string> { "Individual" };
        var mockAccessToken = JwtTokenHelper.GenerateJwtToken(claims, "https://api.pingone.com");

        var mockIdentity = new Mock<ClaimsIdentity>();
        mockIdentity.Setup(identity => identity.Claims).Returns(claims);
        mockIdentity.Setup(identity => identity.IsAuthenticated).Returns(true);

        var user = new ClaimsPrincipal(mockIdentity.Object);

        var mockServiceProvider = JwtTokenHelper.GetMockServiceProviderWithAccessToken(user, mockAccessToken);

        var userController = new SubaccountController(userManager.Object, mockConfigManager.Object, null)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user,
                    RequestServices = mockServiceProvider.Object
                },
            }
        };

        // Act
        var result = await userController.ResendSubaccountActivationLinkAsync(emailParameter, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }
}
