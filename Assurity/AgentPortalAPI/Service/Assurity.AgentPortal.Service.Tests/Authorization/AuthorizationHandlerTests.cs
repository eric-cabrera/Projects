namespace Assurity.AgentPortal.Service.Tests.Authorization;

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Assurity.AgentPortal.Contracts.AgentContracts;
using Assurity.AgentPortal.Service.Authorization;
using Assurity.AgentPortal.Utilities.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class AuthorizationHandlerTests
{
    [Fact]
    public async Task PendingActiveTerminated_IssuerIsPing_ShouldSuceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "AAXB";
        var agentIdClaim = new Claim("agentId", agentId, null, "PingOne");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { agentIdClaim }));
        var mockLogger = new Mock<ILogger<AssurityAuthorizationHandler>>();

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupPingOneUser(agentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.PendingActiveTerminatedRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task PendingActiveTerminated_IssuerIsAzureNotImpersonating_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var claim = new Claim("sub", "abc123", null, "AzureAd");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { claim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupHomeOfficeUser();

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.PendingActiveTerminatedRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task PendingActiveTerminated_Subaccount_ShouldSuceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { RoleConstants.PendingActiveTerminatedRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.PendingActiveTerminatedRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task PendingActiveTerminated_SubaccountWrongRole_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { "DifferentRole" };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.PendingActiveTerminatedRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task PendingActiveTerminated_NoParentAgentClaim_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var roles = new List<string> { RoleConstants.PendingActiveTerminatedRole };
        var claims = new List<Claim> { agentIdClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(string.Empty);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.PendingActiveTerminatedRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task CommissionsAndDebt_IssuerIsPing_ShouldSuceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "AAXB";
        var agentIdClaim = new Claim("agentId", agentId, null, "PingOne");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { agentIdClaim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupPingOneUser(agentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.CommissionsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task CommissionsAndDebt_IssuerIsAzureNotImpersonating_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var claim = new Claim("sub", "abc123", null, "AzureAd");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { claim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupHomeOfficeUser();

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.CommissionsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task CommissionsAndDebt_Subaccount_ShouldSuceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { RoleConstants.CommissionsRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.CommissionsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task CommissionsAndDebt_SubaccountWrongRole_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { "DifferentRole", RoleConstants.PendingActiveTerminatedRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.CommissionsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task CommissionsAndDebt_NoParentAgentClaim_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var roles = new List<string> { RoleConstants.CommissionsRole };
        var claims = new List<Claim> { agentIdClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(string.Empty);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.CommissionsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task ProductionCredit_IssuerIsPing_ShouldSuceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "AAXB";
        var agentIdClaim = new Claim("agentId", agentId, null, "PingOne");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { agentIdClaim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupPingOneUser(agentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ProductionCreditRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task ProductionCredit_IssuerIsAzureNotImpersonating_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var claim = new Claim("sub", "abc123", null, "AzureAd");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { claim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupHomeOfficeUser();

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ProductionCreditRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task ProductionCredit_Subaccount_ShouldSuceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { RoleConstants.ProductionCreditRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ProductionCreditRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task ProductionCredit_SubaccountWrongRole_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { "DifferentRole", RoleConstants.PendingActiveTerminatedRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ProductionCreditRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task ProductionCredit_NoParentAgentClaim_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var roles = new List<string> { RoleConstants.ProductionCreditRole };
        var claims = new List<Claim> { agentIdClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(string.Empty);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ProductionCreditRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task ListBill_IssuerIsAzureNotImpersonating_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var claim = new Claim("sub", "abc123", null, "AzureAd");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { claim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupHomeOfficeUser();

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ListBillRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task ListBill_IssuerIsPing_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "AAXB";
        var agentIdClaim = new Claim("agentId", "AAXB", null, "PingOne");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { agentIdClaim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupPingOneUser(agentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ListBillRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task ListBill_NoParentAgentClaim_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var roles = new List<string> { RoleConstants.ListBillRole };
        var claims = new List<Claim> { agentIdClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(string.Empty);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ListBillRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task ListBill_Subaccount_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { RoleConstants.ListBillRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ListBillRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task ListBill_SubaccountWrongRole_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { "DifferentRole", RoleConstants.PendingActiveTerminatedRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ListBillRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task Claims_IssuerIsAzureNotImpersonating_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var claim = new Claim("sub", "abc123", null, "AzureAd");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { claim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupHomeOfficeUser();

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ClaimsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task Claims_IssuerIsPing_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "AAXB";
        var agentIdClaim = new Claim("agentId", agentId, null, "PingOne");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { agentIdClaim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupPingOneUser(agentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ClaimsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task Claims_NoParentAgentClaim_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var roles = new List<string> { RoleConstants.ClaimsRole };
        var claims = new List<Claim> { agentIdClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(string.Empty);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ClaimsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task Claims_Subaccount_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { RoleConstants.ClaimsRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ClaimsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task Claims_SubaccountWrongRole_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { "DifferentRole", RoleConstants.PendingActiveTerminatedRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ClaimsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task TaxForms_IssuerIsAzureNotImpersonating_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var claim = new Claim("sub", "abc123", null, "AzureAd");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { claim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupHomeOfficeUser();

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.TaxFormsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task TaxForms_IssuerIsPing_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "AAXB";
        var agentIdClaim = new Claim("agentId", agentId, null, "PingOne");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { agentIdClaim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupPingOneUser(agentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.TaxFormsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task TaxForms_NoParentAgentClaim_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var roles = new List<string> { RoleConstants.TaxFormsRole };
        var claims = new List<Claim> { agentIdClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(string.Empty);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.TaxFormsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task TaxForms_Subaccount_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { RoleConstants.TaxFormsRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.TaxFormsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task TaxForms_SubaccountWrongRole_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { "DifferentRole", RoleConstants.PendingActiveTerminatedRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.TaxFormsRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task Contracting_IssuerIsAzureNotImpersonating_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var claim = new Claim("sub", "abc123", null, "AzureAd");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { claim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupHomeOfficeUser();

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ContractingRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task Contracting_IssuerIsPing_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "AAXB";
        var agentIdClaim = new Claim("agentId", agentId, null, "PingOne");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { agentIdClaim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupPingOneUser(agentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ContractingRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task Contracting_NoParentAgentClaim_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var roles = new List<string> { RoleConstants.ContractingRole };
        var claims = new List<Claim> { agentIdClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(string.Empty);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ContractingRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task Contracting_Subaccount_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { RoleConstants.ContractingRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ContractingRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task Contracting_SubaccountWrongRole_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { "DifferentRole", RoleConstants.PendingActiveTerminatedRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.ContractingRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task Hierarchy_IssuerIsAzureNotImpersonating_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var claim = new Claim("sub", "abc123", null, "AzureAd");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { claim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupHomeOfficeUser();

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.HierarchyRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task Hierarchy_IssuerIsPing_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "AAXB";
        var agentIdClaim = new Claim("agentId", agentId, null, "PingOne");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { agentIdClaim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupPingOneUser(agentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.HierarchyRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task Hierarchy_NoParentAgentClaim_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var roles = new List<string> { RoleConstants.HierarchyRole };
        var claims = new List<Claim> { agentIdClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(string.Empty);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.HierarchyRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task Hierarchy_Subaccount_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { RoleConstants.HierarchyRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.HierarchyRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task Hierarchy_SubaccountWrongRole_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { "DifferentRole", RoleConstants.PendingActiveTerminatedRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.HierarchyRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task WorksiteParticipation_IssuerIsAzureNotImpersonating_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var claim = new Claim("sub", "abc123", null, "AzureAd");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { claim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupHomeOfficeUser();

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.WorksiteParticipationRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task WorksiteParticipation_IssuerIsPing_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "AAXB";
        var agentIdClaim = new Claim("agentId", agentId, null, "PingOne");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { agentIdClaim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupPingOneUser(agentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.WorksiteParticipationRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task WorksiteParticipation_NoParentAgentClaim_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var roles = new List<string> { RoleConstants.WorksiteParticipationRole };
        var claims = new List<Claim> { agentIdClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(string.Empty);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.WorksiteParticipationRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task WorksiteParticipation_Subaccount_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { RoleConstants.WorksiteParticipationRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.WorksiteParticipationRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task WorksiteParticipation_SubaccountWrongRole_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { "DifferentRole", RoleConstants.PendingActiveTerminatedRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.WorksiteParticipationRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task CaseManagement_IssuerIsAzureNotImpersonating_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var claim = new Claim("sub", "abc123", null, "AzureAd");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { claim }));

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.CaseManagementRole);

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupHomeOfficeUser();

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task CaseManagement_IssuerIsPing_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "AAXB";
        var agentIdClaim = new Claim("agentId", agentId, null, "PingOne");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { agentIdClaim }));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupPingOneUser(agentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.CaseManagementRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task CaseManagement_NoParentAgentClaim_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var roles = new List<string> { RoleConstants.CaseManagementRole };
        var claims = new List<Claim> { agentIdClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(string.Empty);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.CaseManagementRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task CaseManagement_Subaccount_ShouldSucceed()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { RoleConstants.CaseManagementRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.CaseManagementRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task CaseManagement_SubaccountWrongRole_ShouldFail()
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var roles = new List<string> { "DifferentRole", RoleConstants.PendingActiveTerminatedRole };
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.CaseManagementRole);
        var context = new AuthorizationHandlerContext(new List<IAuthorizationRequirement> { requirement }, user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Theory]
    [InlineData(AccessLevel.Full, true, true)]
    [InlineData(AccessLevel.Full, false, true)]
    [InlineData(AccessLevel.Historical, true, true)]
    [InlineData(AccessLevel.Historical, false, false)]
    public async Task AgentCenter_CheckAgentReportAccess(AccessLevel agentAccessLevel, bool reportHasHistoricalAccess, bool userHasAccess)
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, "PingOne");
        var claims = new List<Claim> { agentIdClaim };
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupPingOneUser(agentId);
        mockHttpContextData.AgentAccessLevel = agentAccessLevel;

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.PendingActiveTerminatedRole, reportHasHistoricalAccess);
        var context = new AuthorizationHandlerContext([requirement], user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.Equal(context.HasSucceeded, userHasAccess);
    }

    [Theory]
    [InlineData(AccessLevel.Full, true, true)]
    [InlineData(AccessLevel.Full, false, true)]
    [InlineData(AccessLevel.Historical, true, true)]
    [InlineData(AccessLevel.Historical, false, false)]
    public async Task AgentCenter_CheckSubaccountParentAgentReportAccess(AccessLevel agentAccessLevel, bool reportHasHistoricalAccess, bool userHasAccess)
    {
        // Arrange
        var mockConfigurationManager = GetMockConfigurationManager(true, true);

        var agentId = "subaccount";
        var issuer = "PingOne";
        var parentAgentId = "1234";
        var agentIdClaim = new Claim("agentId", agentId, null, issuer);
        var parentAgentClaim = new Claim("parentAgentId", parentAgentId, null, issuer);
        var claims = new List<Claim> { agentIdClaim, parentAgentClaim };

        var roles = new List<string> { RoleConstants.PendingActiveTerminatedRole };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role, null, issuer));
        }

        var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

        var mockHttpContextData = new MockHttpContextData();
        mockHttpContextData.SetupSubaccount(parentAgentId);
        mockHttpContextData.AgentAccessLevel = agentAccessLevel;

        var mockHttpContext = GetMockHttpContext(mockHttpContextData.Items, user);

        var requirement = new AssurityAuthorizationRequirement(RoleConstants.PendingActiveTerminatedRole, reportHasHistoricalAccess);
        var context = new AuthorizationHandlerContext([requirement], user, mockHttpContext.Object);
        var handler = GetAssurityAuthorizationHandler(mockConfigurationManager);

        // Act
        await handler.HandleAsync(context);

        // Assert
        Assert.Equal(context.HasSucceeded, userHasAccess);
    }

    private static Mock<HttpContext> GetMockHttpContext(Dictionary<object, object> mockHttpContextDataItems, ClaimsPrincipal user)
    {
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext
            .Setup(httpContext => httpContext.Items)
            .Returns(mockHttpContextDataItems);
        mockHttpContext
            .Setup(httpContext => httpContext.User)
            .Returns(user);

        return mockHttpContext;
    }

    private static AssurityAuthorizationHandler GetAssurityAuthorizationHandler(Mock<IConfigurationManager> mockConfigManager)
    {
        var mockLogger = new Mock<ILogger<AssurityAuthorizationHandler>>();
        return new AssurityAuthorizationHandler(mockLogger.Object);
    }

    private static Mock<IConfigurationManager> GetMockConfigurationManager(bool includeAzureAd, bool includePingOne)
    {
        var mockConfigurationManager = new Mock<IConfigurationManager>();

        if (includeAzureAd)
        {
            mockConfigurationManager.Setup(configurationManager => configurationManager.AzureAdIssuer).Returns("AzureAd");
        }

        if (includePingOne)
        {
            mockConfigurationManager.Setup(configurationManager => configurationManager.PingOneIssuer).Returns("PingOne");
        }

        return mockConfigurationManager;
    }
}
