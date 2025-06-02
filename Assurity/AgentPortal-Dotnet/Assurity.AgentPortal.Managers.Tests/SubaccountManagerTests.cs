namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors;
using Assurity.AgentPortal.Accessors.ProfilePingDTOs;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs.AgentCenterAPIResponses;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;
using Assurity.AgentPortal.Contracts;
using Assurity.AgentPortal.Contracts.SubaccountUsers;
using Assurity.AgentPortal.Managers;
using Assurity.AgentPortal.Managers.Mapping.SubaccountUsers;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

[ExcludeFromCodeCoverage]
public class SubaccountManagerTests
{
    private readonly Mock<ISubaccountAccessor> mockUserAccessor;
    private readonly Mock<ILogger<SubaccountManager>> mockLogger;

    public SubaccountManagerTests()
    {
        mockUserAccessor = new Mock<ISubaccountAccessor>();
        mockLogger = new Mock<ILogger<SubaccountManager>>();
    }

    [Fact]
    public async Task GetSubAccounts_Success()
    {
        // Arrange
        var parentAgentId = "ABCD";
        var userCountUrl = 10;
        var fakeAccessToken = "fakeAccessToken";

        var additionalAgentIds = new Contracts.AdditionalAgentIds
        {
            AgentIds = new List<string>
            {
                parentAgentId,
                "EFGH",
            }
        };

        var mockUsersResponseDTO = new UsersResponseDTO
        {
            UserId = "683ee8b7-ce5f-4b65-b708368b8f",
            Email = "tester@gmail.com",
            Enabled = true,
            SubaccountData = new SubAccountDataDTO
            {
                ParentAgentId = "ABCD",
                Roles = new List<SubAccountRole>
                    {
                        SubAccountRole.ListBill,
                        SubAccountRole.Claims
                    }
            }
        };

        var mockAgentsResponse = new SubAccountResponseDTO
        {
            Embedded = new EmbeddedUsersDTO
            {
                Users = new List<UsersResponseDTO> { mockUsersResponseDTO }
            },
            Links = new LinksDTO
            {
                Next = new LinkDTO { Href = null }
            }
        };
        var mockMapper = new Mock<IMapper>();
        mockUserAccessor.Setup(accessor => accessor.GetAllPingSubaccountsAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockAgentsResponse)
            .Verifiable();

        var mockUserDataAccessor = new Mock<IUserDataAccessor>();
        mockUserDataAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<string?>()))
            .ReturnsAsync(additionalAgentIds);

        var userManager = new SubaccountManager(mockLogger.Object, mockUserAccessor.Object, mockUserDataAccessor.Object, mockMapper.Object);

        // Act
        var response = await userManager.GetSubaccountsAsync(parentAgentId, fakeAccessToken, CancellationToken.None, false);

        // Assert
        mockUserAccessor.Verify(acc => acc.GetAllPingSubaccountsAsync(null, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task ActivateSubaccount_Success()
    {
        // Arrange
        var email = "tester@testy.com";
        var username = "testusername";
        var parentAgentId = "ABCD";
        var parentUserName = "parent";
        var passWord = "password";
        var roles = new List<SubAccountRole>
        {
            SubAccountRole.ListBill,
            SubAccountRole.Claims
        };
        var activationId = Guid.NewGuid().ToString();
        var fakeUserId = Guid.NewGuid().ToString();

        var mockResponse = new PingOneResponse()
        {
            Id = fakeUserId,
            Status = "OK",
        };

        var mockConfirmResposne = new PendingSubaccountActivationResponse()
        {
            Valid = true,
            ActivationAttempts = 1,
            Message = null,
            Subaccount = new PendingSubaccount
            {
                Id = Guid.NewGuid().ToString(),
                AgentId = parentAgentId,
                ParentUsername = parentUserName,
                Email = email,
                Roles = roles,
            }
        };

        mockUserAccessor.Setup(accessor => accessor.ConfirmEmailAndGetRolesAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockConfirmResposne)
            .Verifiable();

        mockUserAccessor.Setup(accessor => accessor.CreatePingSubaccountAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<List<SubAccountRole>>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockResponse)
            .Verifiable();

        mockUserAccessor.Setup(accessor => accessor.CreatePingSubaccountMFADeviceAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .Verifiable();

        mockUserAccessor.Setup(accessor => accessor.DeletePendingSubaccountAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .Verifiable();

        var pingManager = new SubaccountManager(null, mockUserAccessor.Object, null, null);

        // Act
        var response = await pingManager.ActivateSubaccountAsync(
            email,
            activationId,
            passWord,
            CancellationToken.None);

        // Assert
        Assert.NotNull(response);

        mockUserAccessor.Verify(
            acc => acc.ConfirmEmailAndGetRolesAsync(
            email,
            activationId,
            CancellationToken.None),
            Times.Once());

        mockUserAccessor.Verify(
            acc => acc.CreatePingSubaccountAsync(
            email,
            parentAgentId,
            parentUserName,
            roles,
            passWord,
            CancellationToken.None),
            Times.Once());

        mockUserAccessor.Verify(
            acc => acc.CreatePingSubaccountMFADeviceAsync(
            fakeUserId,
            email,
            CancellationToken.None),
            Times.Once());

        mockUserAccessor.Verify(
            acc => acc.DeletePendingSubaccountAsync(
            mockConfirmResposne.Subaccount.Id,
            CancellationToken.None),
            Times.Once());
    }

    [Fact]
    public async Task ActivateSubaccount_ReturnsValidFalse_WhenCreatePingSubaccountResponseIsNull()
    {
        // Arrange
        var email = "tester@testy.com";
        var userId = "AC2021";
        var roles = new List<SubAccountRole>
        {
            SubAccountRole.ListBill,
            SubAccountRole.Claims
        };
        var parentAgentId = "ABCD";
        var userName = "testusername";
        var parentUserName = "parent";
        var passWord = "password";
        var activationId = Guid.NewGuid().ToString();

        var mockResponse = new PingOneResponse();

        var mockConfirmResposne = new PendingSubaccountActivationResponse()
        {
            Valid = true,
            ActivationAttempts = 1,
            Message = null,
            Subaccount = new PendingSubaccount
            {
                Id = Guid.NewGuid().ToString(),
                AgentId = parentAgentId,
                ParentUsername = parentUserName,
                Email = email,
                Roles = roles,
            }
        };

        mockUserAccessor.Setup(accessor => accessor.ConfirmEmailAndGetRolesAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockConfirmResposne)
            .Verifiable();

        mockUserAccessor.Setup(accessor => accessor.CreatePingSubaccountAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<List<SubAccountRole>>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((PingOneResponse)null)
            .Verifiable();
        var pingManager = new SubaccountManager(null, mockUserAccessor.Object, null, null);

        // Act
        var response = await pingManager.ActivateSubaccountAsync(
            userId,
            activationId,
            passWord,
            CancellationToken.None);

        // Assert
        Assert.False(response.Valid);
    }

    [Theory]
    [InlineData("ABCD")]
    [InlineData("EFGH")]
    public async Task DeleteSubAccount_Success_ReturnsTrue(string subaccountParentAgentId)
    {
        // Arrange
        var userId = "1";
        var agentIds = new List<string>
        {
            "ABCD", "EFGH"
        };

        var userResponse = new UsersResponseDTO
        {
            UserId = userId,
            AgentId = "subaccount",
            SubaccountData = new SubAccountDataDTO
            {
                ParentAgentId = subaccountParentAgentId,
            }
        };

        var additionalAgentIds = new AdditionalAgentIds
        {
            AgentIds = agentIds
        };

        mockUserAccessor.Setup(accessor => accessor.DeletePingAccountAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .Verifiable();

        mockUserAccessor.Setup(accessor => accessor.GetPingUser(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userResponse);

        var mockUserDataAccessor = new Mock<IUserDataAccessor>();
        mockUserDataAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<string?>()))
            .ReturnsAsync(additionalAgentIds);

        var pingManager = new SubaccountManager(null, mockUserAccessor.Object, mockUserDataAccessor.Object, null);

        // Act
        var response = await pingManager.DeleteSubaccountAsync(userId, agentIds.First(), true, "123142353", CancellationToken.None);

        // Assert
        Assert.True(response);
    }

    [Fact]
    public async Task DeleteSubAccount_Failure_ReturnsFalse()
    {
        var userName = "Test@test.com";
        var parentAgentId = "ABCD";

        var additionalAgentIds = new AdditionalAgentIds
        {
            AgentIds = new List<string>
            {
                parentAgentId,
                "EFGH",
            }
        };

        mockUserAccessor.Setup(accessor => accessor.DeletePingAccountAsync(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(false)
            .Verifiable();

        var mockUserDataAccessor = new Mock<IUserDataAccessor>();
        mockUserDataAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<string?>()))
            .ReturnsAsync(additionalAgentIds);

        var pingManager = new SubaccountManager(mockLogger.Object, mockUserAccessor.Object, mockUserDataAccessor.Object, null);

        // Act
        var response = await pingManager.DeleteSubaccountAsync(userName, parentAgentId, true, "1234523423", CancellationToken.None);

        // Assert
        Assert.False(response);
    }

    [Theory]
    [InlineData("ABCD")]
    [InlineData("EFGH")]
    public async Task UpdateSubAccount_Success_PingMethodCalled_ReturnsObject(string subaccountParentAgentId)
    {
        var email = "Test@test.com";
        var userId = Guid.NewGuid().ToString();
        var agentIds = new List<string>
        {
            "ABCD",
            "EFGH",
        };

        var parentUserName = "unABCD";
        var roles = new List<SubAccountRole>() { SubAccountRole.TaxForms, SubAccountRole.ListBill };
        var fakeAccessToken = "fakeAccessToken";

        var mockPingResponse = new UsersResponseDTO()
        {
            UserId = userId,
            AgentId = "subaccount",
            SubaccountData = new SubAccountDataDTO()
            {
                ParentAgentId = subaccountParentAgentId,
                ParentUsername = parentUserName,
                Roles = roles,
            }
        };

        var additionalAgentIds = new AdditionalAgentIds
        {
            AgentIds = agentIds
        };

        mockUserAccessor.Setup(accessor => accessor.UpdatePingSubaccountAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<List<SubAccountRole>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockPingResponse)
            .Verifiable();

        mockUserAccessor.Setup(accessor => accessor.GetPingUser(mockPingResponse.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockPingResponse)
            .Verifiable();

        var mockUserDataAccessor = new Mock<IUserDataAccessor>();
        mockUserDataAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(
            It.IsAny<string>(),
            It.IsAny<string?>()))
            .ReturnsAsync(additionalAgentIds);

        var pingManager = new SubaccountManager(null, mockUserAccessor.Object, mockUserDataAccessor.Object, null);

        // Act
        var response = await pingManager.UpdateSubaccountAsync(email, userId, agentIds.First(), parentUserName, roles, true, fakeAccessToken, CancellationToken.None);

        // Assert
        mockUserAccessor.Verify(
            acc => acc.UpdatePingSubaccountAsync(
            userId,
            email,
            agentIds.First(),
            parentUserName,
            roles,
            CancellationToken.None),
            Times.Once());

        mockUserAccessor.Verify(
            acc => acc.GetPingUser(userId, CancellationToken.None), Times.Once);

        Assert.NotNull(response);
        Assert.Equal(mockPingResponse.UserId, response.UserId);
        Assert.Equal([.. mockPingResponse.SubaccountData.Roles.Select(role => role.ToString())], response.Roles);
    }

    [Fact]
    public async Task UpdateSubAccount_Success_PendingMethodCalled_ReturnsObject()
    {
        var email = "Test@test.com";
        var userId = Guid.NewGuid().ToString();
        var parentAgentId = "ABCD";
        var parentUserName = "unABCD";
        var roles = new List<SubAccountRole>() { SubAccountRole.TaxForms, SubAccountRole.ListBill };
        var faceAccessToken = "fakeAccessToken";

        var mockPingResponse = new UsersResponseDTO()
        {
            UserId = userId,
            SubaccountData = new SubAccountDataDTO()
            {
                ParentAgentId = parentAgentId,
                ParentUsername = parentUserName,
                Roles = roles,
            }
        };

        mockUserAccessor.Setup(accessor => accessor.UpdatePendingSubaccountAsync(
            It.IsAny<string>(),
            It.IsAny<List<SubAccountRole>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .Verifiable();

        var pingManager = new SubaccountManager(null, mockUserAccessor.Object, null, null);

        // Act
        var response = await pingManager.UpdateSubaccountAsync(email, userId, parentAgentId, parentUserName, roles, false, faceAccessToken, CancellationToken.None);

        // Assert
        mockUserAccessor.Verify(
            acc => acc.UpdatePendingSubaccountAsync(
            email,
            roles,
            faceAccessToken,
            parentAgentId,
            CancellationToken.None),
            Times.Once());

        Assert.NotNull(response);
        Assert.Equal(mockPingResponse.UserId, response.UserId);
        Assert.Equal(0, response.Roles.Count);
    }

    [Fact]
    public async Task CreatePendingSubaccount_Success_PendingMethodCalled_ReturnsObject()
    {
        var email = "Test@test.com";
        var userId = Guid.NewGuid().ToString();
        var userName = "Test@test.com";
        var parentAgentId = "ABCD";
        var parentUserName = "unABCD";
        var roles = new List<SubAccountRole>() { SubAccountRole.TaxForms, SubAccountRole.ListBill };
        var faceAccessToken = "fakeAccessToken";

        var mockResponse = new PendingSubaccount()
        {
            Id = userId,
            AgentId = parentAgentId,
            ParentUsername = parentUserName,
            Email = email,
            Roles = roles,
        };

        mockUserAccessor.Setup(accessor => accessor.CreatePendingSubaccountAsync(
            It.IsAny<string>(),
            It.IsAny<List<SubAccountRole>>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()))
            .ReturnsAsync(mockResponse)
            .Verifiable();

        var mapperConfiguration = new MapperConfiguration(
            configuration =>
            {
                configuration.AddProfile(typeof(GetSubaccountUsersMappingProfile));
            });

        var mapper = mapperConfiguration.CreateMapper();

        var pingManager = new SubaccountManager(null, mockUserAccessor.Object, null, mapper);

        // Act
        var response = await pingManager.CreatePendingSubaccountAsync(email, roles, faceAccessToken, parentAgentId, CancellationToken.None);

        // Assert
        mockUserAccessor.Verify(
            acc => acc.CreatePendingSubaccountAsync(
            email,
            roles,
            faceAccessToken,
            It.IsAny<string>(),
            CancellationToken.None,
            It.IsAny<string>()),
            Times.Once());

        Assert.NotNull(response);
        Assert.Equal(mockResponse.Id, response.Subaccount.UserId);
        Assert.Equal(mockResponse.Email, response.Subaccount.UserName);
        Assert.Equal(mockResponse.Email, response.Subaccount.Email);
        Assert.False(response.Subaccount.ActivationStatus);
        Assert.Equal(mockResponse.Roles, response.Subaccount.Roles);
    }
}
