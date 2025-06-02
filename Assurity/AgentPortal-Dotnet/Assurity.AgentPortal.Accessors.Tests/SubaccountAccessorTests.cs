namespace Assurity.AgentPortal.Accessors.Tests;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs;
using Assurity.AgentPortal.Accessors.SubAccount.DTOs.PingResponses;
using Assurity.AgentPortal.Contracts.SubaccountUsers;
using Assurity.AgentPortal.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

[ExcludeFromCodeCoverage]
public class SubaccountAccessorTests
{
    private readonly Uri baseAddress = new Uri("http://localhost");

    private readonly Uri pingBaseUrl = new Uri("https://api.pingone.com/v1");

    [Fact]
    public async Task CreatePingSubAccount_Success()
    {
        // Arrange
        var email = "testemail.com";
        var roles = new List<SubAccountRole>
        {
            SubAccountRole.ListBill,
            SubAccountRole.Claims
        };
        var parentAgentId = "ABCD";
        var username = "testusername";
        var parentUserName = "parent";
        var passWord = "password";
        var mockResponse = new UserDTO()
        {
            Email = email,
            CreatedAt = DateTime.Now.Date,
            UpdatedAt = DateTime.Now.Date,
            Name = new NameDTO
            {
                Given = "Sub",
                Family = "Account"
            },
            Username = $"{username}",
            Population = new PopulationDTO
            {
                Id = "a980cdf3-0fa9-47c3-b304-8c33e3b3d55a"
            },
            SubaccountData = new SubAccountDataDTO
            {
                ParentUsername = "unABCD",
                ParentAgentId = "ABCD",
                Roles = roles
            }
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.Created);
        var userAccessor = GetUserAccessor(mockHttpMessageHandler);

        // Act
        var response = await userAccessor.CreatePingSubaccountAsync(email, parentAgentId, parentUserName, roles, passWord, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetPingSubAccounts_Success_CustomUrl()
    {
        // Arrange
        var email = "testemail.com";
        var firstName = "Test";
        var lastName = "User";
        var roles = new List<SubAccountRole>
        {
            SubAccountRole.ListBill,
            SubAccountRole.Claims,
        };
        var agentId = "ABCD";
        string username = email.Split('@')[0];
        var parentUserName = "un" + agentId;
        var userCountUrl = pingBaseUrl + "/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users?filter=agentID eq \"" + agentId + "\"&limit=10";

        var mockResponse = new SubAccountResponseDTO
        {
            Links = new LinksDTO
            {
                Self = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users?limit=200&filter=agentID%20eq%20%22AAXB%22" }
            },
            Embedded = new EmbeddedUsersDTO
            {
                Users = new List<UsersResponseDTO>
                {
                    new UsersResponseDTO
                    {
                        Links = new LinksDTO
                        {
                            Password = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f/password" },
                            PasswordSet = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f/password" },
                            AccountSendVerificationCode = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f" },
                            LinkedAccounts = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f/linkedAccounts" },
                            Self = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f" },
                            PasswordCheck = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f/password" },
                            PasswordReset = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f/password" },
                            PasswordRecover = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f/password" }
                        },
                        UserId = "683ee8b7-ce5f-4b65-b845-367708368b8f",
                        Environment = new EnvironmentDTO { Id = "e6a74c91-fd47-406c-8da5-a2d804d6bf2c" },
                        Account = new AccountDTO { CanAuthenticate = true, Status = "OK" },
                        CreatedAt = DateTime.Parse("2021-07-01T21:27:58.217Z"),
                        Email = "tester@gmail.com",
                        Enabled = true,
                        IdentityProvider = new IdentityProviderDTO { Type = "PING_ONE" },
                        Lifecycle = new LifecycleDTO { Status = "ACCOUNT_OK" },
                        MfaEnabled = true,
                        Population = new PopulationDTO { Id = "e62e16ea-310c-436e-9a7b-e7c1091441da" },
                        SubaccountData = new SubAccountDataDTO { Roles = roles },
                        UpdatedAt = DateTime.Parse("2024-12-13T16:56:16.570Z"),
                        UserName = "unAAXB",
                        VerifyStatus = "NOT_INITIATED"
                    }
                }
            },
            Count = 1,
            Size = 1
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.OK);
        var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object)
        {
            BaseAddress = baseAddress
        };
        var pingAccessor = GetUserAccessor(mockHttpMessageHandler);

        // Act
        var result = await pingAccessor.GetAllPingSubaccountsAsync(null, CancellationToken.None);

        // Assert
        Assert.Equal(1, result.Count);
    }

    [Fact]
    public async Task GetPingSubAccounts_IfUnsuccessful_ShouldReturnNull()
    {
        // Arrange
        var agentId = "AC2021";
        var mockHttpMessageHandler = GetMockHttpMessageHandler(null, HttpStatusCode.InternalServerError);
        var pingAccessor = GetUserAccessor(mockHttpMessageHandler);

        // Act
        var result = await pingAccessor.GetAllPingSubaccountsAsync(null, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetPingSubAccounts_Success_IfCustomUrl_IsNull()
    {
        // Arrange
        var email = "testemail.com";
        var firstName = "Test";
        var lastName = "User";
        var roles = new List<SubAccountRole>
        {
            SubAccountRole.ListBill,
            SubAccountRole.Claims
        };
        var agentId = "ABCD";
        string username = email.Split('@')[0];
        var parentUserName = "un" + agentId;

        var mockResponse = new SubAccountResponseDTO
        {
            Links = new LinksDTO
            {
                Self = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users?limit=200&filter=agentID%20eq%20%22AAXB%22" }
            },
            Embedded = new EmbeddedUsersDTO
            {
                Users = new List<UsersResponseDTO>
                {
                    new UsersResponseDTO
                    {
                        Links = new LinksDTO
                        {
                            Password = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f/password" },
                            PasswordSet = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f/password" },
                            AccountSendVerificationCode = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f" },
                            LinkedAccounts = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f/linkedAccounts" },
                            Self = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f" },
                            PasswordCheck = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f/password" },
                            PasswordReset = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f/password" },
                            PasswordRecover = new LinkDTO { Href = "https://api.pingone.com/v1/environments/e6a74c91-fd47-406c-8da5-a2d804d6bf2c/users/683ee8b7-ce5f-4b65-b845-367708368b8f/password" }
                        },
                        UserId = "683ee8b7-ce5f-4b65-b845-367708368b8f",
                        Environment = new EnvironmentDTO { Id = "e6a74c91-fd47-406c-8da5-a2d804d6bf2c" },
                        Account = new AccountDTO { CanAuthenticate = true, Status = "OK" },
                        CreatedAt = DateTime.Parse("2021-07-01T21:27:58.217Z"),
                        Email = "tester@gmail.com",
                        Enabled = true,
                        IdentityProvider = new IdentityProviderDTO { Type = "PING_ONE" },
                        Lifecycle = new LifecycleDTO { Status = "ACCOUNT_OK" },
                        MfaEnabled = true,
                        Population = new PopulationDTO { Id = "e62e16ea-310c-436e-9a7b-e7c1091441da" },
                        SubaccountData = new SubAccountDataDTO { Roles = roles },
                        UpdatedAt = DateTime.Parse("2024-12-13T16:56:16.570Z"),
                        UserName = "unAAXB",
                        VerifyStatus = "NOT_INITIATED"
                    }
                }
            },
            Count = 1,
            Size = 1
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.OK);
        var pingAccessor = GetUserAccessor(mockHttpMessageHandler);

        // Act
        var result = await pingAccessor.GetAllPingSubaccountsAsync(null, CancellationToken.None);

        // Assert
        Assert.Equal(1, result.Count);
    }

    [Fact]
    public async Task GetPendingSubAccounts_Success()
    {
        // Arrange
        var email = "testemail.com";
        var firstName = "Test";
        var lastName = "User";
        var roles = new List<SubAccountRole>
        {
            SubAccountRole.ListBill,
            SubAccountRole.Claims
        };
        var agentId = "ABCD";
        var parentUserName = "un" + agentId;
        var fakeAccessToken = "fakeAccessToken";

        var mockResponse = new List<PendingSubaccount>
        {
            new PendingSubaccount
            {
                Id = Guid.NewGuid().ToString(),
                AgentId = agentId,
                ParentUsername = parentUserName,
                Email = email,
                Roles = roles,
            }
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.OK);
        var pingAccessor = GetUserAccessor(null, mockHttpMessageHandler);

        // Act
        var result = await pingAccessor.GetPendingSubaccountsAsync(agentId, fakeAccessToken, CancellationToken.None);

        // Assert
        Assert.Equal(1, result.Count);
    }

    [Fact]
    public async Task GetPingUser_ReturnsUserResponse_WhenSuccessStatusCode()
    {
        // Arrange
        var userId = "123";
        var expected = new UsersResponseDTO { UserId = "123", UserName = "john.doe" };

        var responseMessage = GetMockHttpMessageHandler(expected, HttpStatusCode.OK);

        var service = GetUserAccessor(responseMessage);

        // Act
        var result = await service.GetPingUser(userId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.UserId, result!.UserId);
        Assert.Equal(expected.UserName, result.UserName);
    }

    [Fact]
    public async Task GetPingUser_LogsErrorAndReturnsNull_WhenNotSuccessStatusCode()
    {
        // Arrange
        var userId = "123";
        var errorContent = "Not Found";

        var responseMessage = GetMockHttpMessageHandler(errorContent, HttpStatusCode.NotFound);
        var service = GetUserAccessor(responseMessage);

        // Act
        var result = await service.GetPingUser(userId, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    private static Mock<HttpMessageHandler> GetMockHttpMessageHandler(
        object? mockResponse,
        HttpStatusCode statusCode)
    {
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(JsonSerializer.Serialize(mockResponse))
        };

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(httpResponseMessage)
            .Verifiable();

        return mockHttpMessageHandler;
    }

    private SubaccountAccessor GetUserAccessor(Mock<HttpMessageHandler>? mockHttpMessageHandler = null, Mock<HttpMessageHandler>? mockAPIHttpMessageHandler = null)
    {
        HttpClient mockhttpClient = null;

        if (mockHttpMessageHandler != null)
        {
            mockhttpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = baseAddress
            };
        }

        var mockLogger = new Mock<ILogger<SubaccountAccessor>>(MockBehavior.Loose);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var mockConfigManager = new Mock<IConfigurationManager>();

        var mockHttpClientFactory = new Mock<IHttpClientFactory>();

        if (mockAPIHttpMessageHandler != null)
        {
            var apiHttpClient = new HttpClient(mockAPIHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://example.com/")
            };

            mockHttpClientFactory.Setup(factory => factory.CreateClient("AgentPortalAPIHttpClient")).Returns(apiHttpClient);
        }

        return new SubaccountAccessor(mockhttpClient, mockHttpClientFactory.Object, mockConfigManager.Object, mockLogger.Object, memoryCache);
    }
}
