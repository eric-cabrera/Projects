namespace Assurity.AgentPortal.Web.UnitTests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Assurity.AgentPortal.Contracts;
using Assurity.AgentPortal.Managers;
using Assurity.AgentPortal.Utilities;
using Assurity.AgentPortal.Web.Controllers;
using Assurity.AgentPortal.Web.Tests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class AuthControllerTests
{
    [Fact]
    public void Login_ShouldSucceed()
    {
        // Arrange
        var mockConfigManager = new Mock<IConfigurationManager>();

        var authController = new AuthController(null, mockConfigManager.Object, null, null, null);

        // Act
        var result = authController.Login();

        // Assert
        Assert.IsType<ChallengeResult>(result);
    }

    [Fact]
    public void Login_ShouldReturnChallengeResult()
    {
        // Arrange
        var mockConfigManager = new Mock<IConfigurationManager>();
        var controller = new AuthController(null, mockConfigManager.Object, null, null, null);

        // Act
        var result = controller.Login();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ChallengeResult>(result);
    }

    [Fact]
    public void Login_CreateAccountFlag_ShouldSetCookie()
    {
        // Arrange
        var expectedName = ".agentCenter.createFlag";
        var expectedValue = "true";

        var mockHttpContext = new Mock<HttpContext>();
        var responseCookiesMock = new Mock<IResponseCookies>();
        responseCookiesMock.Setup(c => c.Append(expectedName, expectedValue, It.IsAny<CookieOptions>())).Verifiable();
        mockHttpContext.Setup(context => context.Response.Cookies).Returns(responseCookiesMock.Object);

        var mockConfigManager = new Mock<IConfigurationManager>();
        mockConfigManager.Setup(x => x.Environment).Returns("local");

        var controller = new AuthController(null, mockConfigManager.Object, null, null, null)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object,
            },
        };

        // Act
        var result = controller.Login(creatingAccount: true);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ChallengeResult>(result);
        responseCookiesMock.Verify();
    }

    [Fact]
    public void HomeOfficeLogin_AssureLinkCookieSet_ShouldSetCookieAndReturnChallengeResult()
    {
        // Arrange
        var mockHttpContext = new Mock<HttpContext>();

        var requestCookie = new Cookie
        {
            Secure = true,
            HttpOnly = true,
            Domain = ".assurity.com",
            Name = "__Secure-ac-data",
            Value = JsonSerializer.Serialize(new AgentData("username", "Howdy")),
        };

        var expectedName = "__Host-ac-data";
        var expectedValue = requestCookie.Value;

        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        var requestValue = requestCookie.Value;
        mockRequestCookies.Setup(c => c.TryGetValue(requestCookie.Name, out requestValue)).Returns(true);
        mockHttpContext.Setup(context => context.Request.Cookies).Returns(mockRequestCookies.Object);

        var responseCookiesMock = new Mock<IResponseCookies>();
        responseCookiesMock.Setup(c => c.Append(expectedName, expectedValue, It.IsAny<CookieOptions>())).Verifiable();
        mockHttpContext.Setup(context => context.Response.Cookies).Returns(responseCookiesMock.Object);

        var mockConfigManager = new Mock<IConfigurationManager>();
        mockConfigManager.Setup(mockConfigManager => mockConfigManager.Decrypt(It.IsAny<string>(), true))
            .Returns(requestCookie.Value);
        mockConfigManager.Setup(mockConfig => mockConfig.Encrypt(It.IsAny<string>()))
            .Returns<string>(x => x);

        var controller = new AuthController(null, mockConfigManager.Object, null, null, null)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object,
            },
        };

        // Act
        var result = controller.HomeOfficeLogin();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ChallengeResult>(result);
        responseCookiesMock.Verify();
    }

    [Fact]
    public void HomeOfficeLogin_NoCookieSet_ShouldReturnChallengeResult()
    {
        // Arrange
        var mockHttpContext = new Mock<HttpContext>();
        var mockCookieCollection = new Mock<IRequestCookieCollection>();

        string? falseResult = null;
        mockCookieCollection.Setup(mockCookieCollection => mockCookieCollection.TryGetValue(It.IsAny<string>(), out falseResult))
            .Returns(false);
        mockHttpContext.Setup(c => c.Request.Cookies).Returns(mockCookieCollection.Object);

        var mockConfigManager = new Mock<IConfigurationManager>();
        mockConfigManager.Setup(mockConfigManager => mockConfigManager.AssureLinkUrl).Returns("www.assurity.com");

        var controller = new AuthController(null, mockConfigManager.Object, null, null, null)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object,
            },
        };

        // Act
        var result = controller.HomeOfficeLogin();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ChallengeResult>(result);
    }

    [Fact]
    public async Task Logout_PingOneScheme_ShouldSucceed()
    {
        // Arrange
        var claims = new List<Claim>
        {
                new Claim("preferred_username", "unAAXB", string.Empty, "Ping"),
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        Mock<IAuthenticationService> mockIAuthenticationService = new();
        mockIAuthenticationService
            .Setup(x => x.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.FromResult((object)true));
        Mock<IServiceProvider> mockIServiceProvider = new();
        mockIServiceProvider
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(mockIAuthenticationService.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(context => context.User).Returns(user);
        mockHttpContext.Setup(x => x.RequestServices).Returns(mockIServiceProvider.Object);

        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        mockHttpContext.Setup(context => context.Request.Cookies).Returns(mockRequestCookies.Object);

        var mockConfigManager = new Mock<IConfigurationManager>();
        mockConfigManager.Setup(configManager => configManager.AzureAdAuthority).Returns("AzureAd");

        var authController = new AuthController(null, mockConfigManager.Object, null, null, null)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object,
            }
        };

        // Act
        var result = await authController.Logout();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<SignOutResult>(result);
        var signoutResult = (SignOutResult)result;
        Assert.Equal("PingOne", signoutResult.AuthenticationSchemes.Single());
    }

    [Fact]
    public async Task Logout_AzureAdScheme_ShouldSucceed()
    {
        // Arrange
        var claims = new List<Claim>
        {
                new Claim("preferred_username", "unAAXB", string.Empty, "AzureAd"),
        };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockIAuthenticationService = new Mock<IAuthenticationService>();
        mockIAuthenticationService
            .Setup(x => x.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()))
            .Returns(Task.FromResult((object)true));
        var mockIServiceProvider = new Mock<IServiceProvider>();
        mockIServiceProvider
                .Setup(x => x.GetService(typeof(IAuthenticationService)))
                .Returns(mockIAuthenticationService.Object);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(context => context.User).Returns(user);
        mockHttpContext.Setup(x => x.RequestServices).Returns(mockIServiceProvider.Object);

        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        mockHttpContext.Setup(context => context.Request.Cookies).Returns(mockRequestCookies.Object);

        var mockConfigManager = new Mock<IConfigurationManager>();
        mockConfigManager.Setup(configManager => configManager.AzureAdAuthority).Returns("AzureAd");

        var authController = new AuthController(null, mockConfigManager.Object, null, null, null)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object,
            }
        };

        // Act
        var result = await authController.Logout();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<SignOutResult>(result);
        var signoutResult = (SignOutResult)result;
        Assert.Equal("AzureAd", signoutResult.AuthenticationSchemes.Single());
    }

    [Fact]
    public async void GetUser_ShouldSucceed()
    {
        // Arrange
        var username = "unAAXB";
        var agentId = "AAXB";
        var firstName = "John";
        var lastName = "Smith";
        var email = "test@email.com";
        var claims = new List<Claim>
        {
                new Claim("preferred_username", username, string.Empty, "PingOne"),
                new Claim("AgentID", agentId, string.Empty, "AzureAd"),
                new Claim("role", "PendingActiveTerminated", string.Empty, "PingOne"),
                new Claim("given_name", firstName, string.Empty, "PingOne"),
                new Claim("family_name", lastName, string.Empty, "PingOne"),
                new Claim("email", email, string.Empty, "PingOne")
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

        mockServiceProvider.Setup(x => x.GetService(typeof(IConfigurationManager))).Returns(configManager.Object);

        var mockUserDataManager = new Mock<IUserDataManager>();
        mockUserDataManager.Setup(manager => manager.GetProductionCreditBusinessTypes(mockAccessToken, null)).ReturnsAsync(userBusinessTypes);

        var mockFeatureManager = new Mock<IFeatureManager>();
        mockFeatureManager.Setup(manager => manager.IsEnabledAsync("productioncredit"))
            .ReturnsAsync(true);

        var mockLogger = new Mock<ILogger<AuthController>>();

        var authController = new AuthController(mockLogger.Object, configManager.Object, mockUserDataManager.Object, null, mockFeatureManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user,
                    RequestServices = mockServiceProvider.Object,
                    Items = new Dictionary<object, object>()
                    {
                        { "AgentData", null },
                        { "IsHomeOfficeUser", false },
                        { "IsPingUser", true },
                        { "IsSubaccount", false },
                        { "AgentId", agentId },
                        { "Email", email },
                        { "FirstName", firstName },
                        { "LastName", lastName },
                        { "Username", username }
                    }
                },
            }
        };

        // Act
        var actionResult = await authController.GetUser();

        // Assert
        var result = actionResult.Result as ObjectResult;
        var data = result!.Value as UserInfoData;

        Assert.NotNull(data);
        Assert.Equal("unAAXB", data.Username);
        Assert.Equal("AAXB", data.AgentId);
        Assert.Equal("PendingActiveTerminated", data.Roles.First());
        Assert.False(data.IsSubaccount);
        Assert.Equal(userBusinessTypes, data.ProductionCreditBusinessTypes);
        Assert.Equal("John", data.FirstName);
        Assert.Equal("Smith", data.LastName);
        Assert.Equal("test@email.com", data.EmailAddress);
        Assert.True(data.IsPingOneAudience);
    }

    [Fact]
    public async void GetUser_SubAccount_ShouldSucceed()
    {
        // Arrange
        var username = "John Smith";
        var agentId = "subaccount";
        var firstName = "John";
        var lastName = "Smith";
        var email = "test@email.com";
        var parentAgentId = "AAXB";
        var claims = new List<Claim>
        {
                new Claim("preferred_username", username, string.Empty, "PingOne"),
                new Claim("AgentID", agentId, string.Empty, "PingOne"),
                new Claim("role", "PendingActiveTerminated", string.Empty, "PingOne"),
                new Claim("parentAgentId", parentAgentId, string.Empty, "PingOne"),
        };

        var userBusinessTypes = new List<string> { "Individual" };
        var mockAccessToken = JwtTokenHelper.GenerateJwtToken(claims, "not.ping.one");

        var mockIdentity = new Mock<ClaimsIdentity>();
        mockIdentity.Setup(identity => identity.Claims).Returns(claims);
        mockIdentity.Setup(identity => identity.IsAuthenticated).Returns(true);

        var user = new ClaimsPrincipal(mockIdentity.Object);

        var mockServiceProvider = JwtTokenHelper.GetMockServiceProviderWithAccessToken(user, mockAccessToken);

        var configManager = new Mock<IConfigurationManager>();
        configManager.Setup(config => config.PingOneAuthority).Returns("PingOne");

        var mockUserDataManager = new Mock<IUserDataManager>();
        mockUserDataManager.Setup(manager => manager.GetProductionCreditBusinessTypes(mockAccessToken, agentId)).ReturnsAsync(userBusinessTypes);

        var mockFeatureManager = new Mock<IFeatureManager>();
        mockFeatureManager.Setup(manager => manager.IsEnabledAsync("productioncredit"))
            .ReturnsAsync(true);

        var mockLogger = new Mock<ILogger<AuthController>>();

        var authController = new AuthController(mockLogger.Object, configManager.Object, mockUserDataManager.Object, null, mockFeatureManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user,
                    RequestServices = mockServiceProvider.Object,
                    Items = new Dictionary<object, object>
                    {
                        { "AgentData", null },
                        { "IsHomeOfficeUser", false },
                        { "IsPingUser", true },
                        { "IsSubaccount", true },
                        { "AgentId", parentAgentId },
                        { "Email", email },
                        { "FirstName", firstName },
                        { "LastName", lastName },
                        { "Username", username }
                    }
                },
            }
        };

        authController.Request.Headers.Add("agent-id", agentId);

        // Act
        var actionResult = await authController.GetUser();

        // Assert
        var result = actionResult.Result as ObjectResult;
        var data = result!.Value as UserInfoData;

        Assert.NotNull(data);
        Assert.Equal("John Smith", data.Username);
        Assert.Equal("AAXB", data.AgentId);
        Assert.Equal("PendingActiveTerminated", data.Roles.First());
        Assert.True(data.IsSubaccount);
        Assert.False(data.IsPingOneAudience);
    }

    [Fact]
    public async void GetUser_HomeOffice_ShouldSucceed()
    {
        // Arrange
        var agentId = "AAXB";

        var claims = new List<Claim>
        {
            new Claim("preferred_username", "HomeOfficeUser", string.Empty, "AzureAd"),
        };

        var userBusinessTypes = new List<string> { "Individual", "Worksite" };
        var mockAccessToken = JwtTokenHelper.GenerateJwtToken(claims, "https://api.pingone.com");

        var requestCookie = new Cookie
        {
            Secure = true,
            HttpOnly = true,
            Domain = ".assurity.com",
            Name = "__Host-ac-data",
            Value = JsonSerializer.Serialize(new AgentData("Sup", agentId)),
        };

        var requestValue = requestCookie.Value;

        var mockIdentity = new Mock<ClaimsIdentity>();
        mockIdentity.Setup(identity => identity.Claims).Returns(claims);
        mockIdentity.Setup(identity => identity.IsAuthenticated).Returns(true);

        var user = new ClaimsPrincipal(mockIdentity.Object);
        var items = new Dictionary<object, object>()
                    {
                        { "AgentData", null },
                        { "IsHomeOfficeUser", true },
                        { "IsPingUser", false },
                        { "IsSubaccount", false },
                        { "AgentId", agentId },
                        { "Email", string.Empty },
                        { "FirstName", string.Empty },
                        { "LastName", string.Empty },
                        { "Username", "HomeOfficeUser" }

                    };

        var mockServiceProvider = JwtTokenHelper.GetMockServiceProviderWithAccessToken(user, mockAccessToken);

        var configManager = new Mock<IConfigurationManager>();
        configManager.Setup(config => config.AzureAdAuthority).Returns("AzureAd");
        configManager.Setup(config => config.Decrypt(requestValue, false)).Returns(requestValue);

        var mockUserDataManager = new Mock<IUserDataManager>();
        mockUserDataManager.Setup(manager => manager.GetProductionCreditBusinessTypes(mockAccessToken, agentId)).ReturnsAsync(userBusinessTypes);

        var mockFeatureManager = new Mock<IFeatureManager>();
        mockFeatureManager.Setup(manager => manager.IsEnabledAsync("productioncredit"))
            .ReturnsAsync(true);

        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext.Setup(context => context.User).Returns(user);
        mockHttpContext.Setup(x => x.RequestServices).Returns(mockServiceProvider.Object);

        var mockRequestCookies = new Mock<IRequestCookieCollection>();
        mockRequestCookies.Setup(c => c.TryGetValue(requestCookie.Name, out requestValue)).Returns(true);
        mockHttpContext.Setup(context => context.Request.Cookies).Returns(mockRequestCookies.Object);
        mockHttpContext.Setup(context => context.User).Returns(user);
        mockHttpContext.Setup(context => context.RequestServices).Returns(mockServiceProvider.Object);
        mockHttpContext.Setup(context => context.Items).Returns(items);

        var mockLogger = new Mock<ILogger<AuthController>>();

        var authController = new AuthController(mockLogger.Object, configManager.Object, mockUserDataManager.Object, null, mockFeatureManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext.Object,
            }
        };

        // Act
        var actionResult = await authController.GetUser();

        // Assert
        var result = actionResult.Result as ObjectResult;
        var data = result!.Value as UserInfoData;

        Assert.NotNull(data);
        Assert.Equal("HomeOfficeUser", data.Username);
        Assert.Equal(agentId, data.AgentId);
        Assert.False(data.IsSubaccount);
        Assert.Equal(userBusinessTypes, data.ProductionCreditBusinessTypes);
        Assert.True(data.IsHomeOffice);
        Assert.True(data.IsPingOneAudience);
    }

    [Fact]
    public async void GetUser_Agency_ShouldSucceed()
    {
        // Arrange
        var username = "unAAXB";
        var agentId = "AAXB";
        var firstName = "John";
        var lastName = "Agency";
        var email = "test@email.com";
        var claims = new List<Claim>
        {
                new Claim("preferred_username", username, string.Empty, "PingOne"),
                new Claim("AgentID", agentId, string.Empty, "AzureAd"),
                new Claim("role", "PendingActiveTerminated", string.Empty, "PingOne"),
                new Claim("given_name", firstName, string.Empty, "PingOne"),
                new Claim("family_name", lastName, string.Empty, "PingOne"),
                new Claim("email", email, string.Empty, "PingOne")
        };

        var userBusinessTypes = new List<string> { "Individual" };
        var mockAccessToken = JwtTokenHelper.GenerateJwtToken(claims, "notPingone");

        var mockIdentity = new Mock<ClaimsIdentity>();
        mockIdentity.Setup(identity => identity.Claims).Returns(claims);
        mockIdentity.Setup(identity => identity.IsAuthenticated).Returns(true);

        var user = new ClaimsPrincipal(mockIdentity.Object);

        var mockServiceProvider = JwtTokenHelper.GetMockServiceProviderWithAccessToken(user, mockAccessToken);

        var configManager = new Mock<IConfigurationManager>();
        configManager.Setup(config => config.PingOneAuthority).Returns("PingOne");

        var mockUserDataManager = new Mock<IUserDataManager>();
        mockUserDataManager.Setup(manager => manager.GetProductionCreditBusinessTypes(mockAccessToken, null)).ReturnsAsync(userBusinessTypes);

        var mockFeatureManager = new Mock<IFeatureManager>();
        mockFeatureManager.Setup(manager => manager.IsEnabledAsync("productioncredit"))
            .ReturnsAsync(true);

        var mockLogger = new Mock<ILogger<AuthController>>();

        var authController = new AuthController(mockLogger.Object, configManager.Object, mockUserDataManager.Object, null, mockFeatureManager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user,
                    RequestServices = mockServiceProvider.Object,
                    Items = new Dictionary<object, object>()
                    {
                        { "AgentData", null },
                        { "IsHomeOfficeUser", false },
                        { "IsPingUser", true },
                        { "IsSubaccount", false },
                        { "AgentId", agentId },
                        { "Email", email },
                        { "FirstName", firstName },
                        { "LastName", string.Empty },
                        { "Username", username }
                    }
                },
            }
        };

        // Act
        var actionResult = await authController.GetUser();

        // Assert
        var result = actionResult.Result as ObjectResult;
        var data = result!.Value as UserInfoData;

        Assert.NotNull(data);
        Assert.Equal("unAAXB", data.Username);
        Assert.Equal("AAXB", data.AgentId);
        Assert.Equal("PendingActiveTerminated", data.Roles.First());
        Assert.False(data.IsSubaccount);
        Assert.Equal(userBusinessTypes, data.ProductionCreditBusinessTypes);
        Assert.Equal("John", data.FirstName);
        Assert.Equal(string.Empty, data.LastName);
        Assert.Equal("test@email.com", data.EmailAddress);
        Assert.False(data.IsPingOneAudience);
    }
}
