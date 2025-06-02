namespace Assurity.AgentPortal.Service.Tests.Controller
{
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Mime;
    using System.Security.Claims;
    using Assurity.AgentPortal.Contracts.PolicyInfo;
    using Assurity.AgentPortal.Contracts.Shared;
    using Assurity.AgentPortal.Managers.PolicyInfo;
    using Assurity.AgentPortal.Service.Controllers;
    using Assurity.AgentPortal.Utilities.Configs;
    using Assurity.AgentPortal.Utilities.Logging;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using AutoBogus;
    using Bogus;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class PolicyInfoControllerTests
    {
        [Fact]
        public async Task GetPolicyAsync_ShouldReturn_ErrorMessage()
        {
            // Arrange
            var testPolicyNumber = "6653458900";
            var testAgentId = "4580";
            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.GetPolicyInfo(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.GetPolicy(testPolicyNumber);
            var result = actionResult as ObjectResult;

            var details = result.Value as ProblemDetails;

            // Assert
            Assert.Equal("An unexpected error occured.", details.Detail);
            Assert.Equal("InternalServerError", details.Title);
            Assert.Equal(500, details.Status);
        }

        [Fact]
        public async Task GetPolicyAsync_ShouldReturn__PolicyFound()
        {
            // Arrange
            var testPolicyNumber = "6653458900";
            var testAgentId = "4580";

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.GetPolicyInfo(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(() => new PolicyResponse());

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.GetPolicy(testPolicyNumber);
            var okResult = actionResult as OkObjectResult;

            // Assert
            Assert.IsType<PolicyResponse>(okResult.Value);
        }

        [Fact]
        public async Task GetPolicySummariesAsync_ShouldReturn_EmptySummary()
        {
            // Arrange
            var testAgentId = "4580";

            var mockResponse = new PolicySummariesResponse
            {
                Policies = new List<PolicySummary>(),
            };

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.GetPolicySummaries(
                It.IsAny<string>(),
                It.IsAny<Status>(),
                It.IsAny<string>()))
                .ReturnsAsync(mockResponse);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.GetPoliciesByStatus(Status.Terminated);
            var result = actionResult as ObjectResult;

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(mockResponse, result.Value);
        }

        [Fact]
        public async Task GetPolicySummariesAsync_ShouldReturn_PolicySummariesResponse()
        {
            // Arrange
            var testAgentId = "4580";
            var policySummaryFaker = new AutoFaker<PolicySummariesResponse>();
            var mockResponse = policySummaryFaker.Generate();

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.GetPolicySummaries(
                It.IsAny<string>(),
                It.IsAny<Status>(),
                It.IsAny<string>()))
                .ReturnsAsync(mockResponse);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.GetPoliciesByStatus(Status.Terminated);
            var okResult = actionResult as OkObjectResult;

            // Assert
            Assert.IsType<PolicySummariesResponse>(okResult.Value);
        }

        [Fact]
        public async Task GetPolicySumamriesAsync_NoAgentIdPassedIn_ShouldReturnBadRequest()
        {
            // Arrange
            var testAgentId = string.Empty;
            var mockHttpRequsetMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            mockLogger.Setup(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Warning),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var policyInfoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequsetMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var response = await policyInfoController
                .GetPoliciesByStatus(Status.Active);

            // Assert
            Assert.IsType<UnauthorizedResult>(response);
        }

        [Fact]
        public async Task GetPolicySummariesAsync_ExceptionThrown_ShouldReturnInternalServerError()
        {
            // Arrange
            var testAgentId = "4580";
            var mockHttpRequsetMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            mockLogger.Setup(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager
                .Setup(policyInfoManager => policyInfoManager.GetPolicySummaries(
                    It.IsAny<string>(),
                    It.IsAny<Status>(),
                    It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var policyInfoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequsetMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var response = await policyInfoController
                .GetPoliciesByStatus(Status.Active);

            // Assert
            Assert.IsType<ObjectResult>(response);
        }

        [Fact]
        public async Task GetPendingPolicyRequirementSummaries_ShouldReturn_EmptySummary()
        {
            // Arrange
            var testAgentId = "4580";

            var requirementResponse = new RequirementSummariesResponse
            {
                RequirementSummaries = new List<RequirementSummary>(),
            };

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.GetPendingPolicyRequirements(
                It.IsAny<string>(),
                It.IsAny<string?>()))
                .ReturnsAsync(requirementResponse);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.GetPendingPolicyRequirements();
            var result = actionResult as ObjectResult;

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(requirementResponse, result.Value);
        }

        [Fact]
        public async Task GetPendingPolicyRequirementSummaries_ShouldReturn_RequirementSummariesResponse()
        {
            // Arrange
            var testAgentId = "4580";

            var testRequirementSummaries = new RequirementSummariesResponse
            {
                TotalPages = 1,
                TotalRequirements = 10,
                CurrentPage = 1,
                RequirementSummaries = new List<RequirementSummary>
            {
                new RequirementSummary
                {
                    AssignedAgents = new List<AssignedAgent>
                    {
                        new AssignedAgent
                        {
                            AssignedAgentId = "1234"
                        }
                    },
                    EmployerName = "ABC Corporation",
                    PrimaryInsuredName = "John",
                    ProductCategory = "AccidentalDeath",
                    PolicyNumber = "123456789",
                    Requirement = new RequirementResponse
                    {
                        Id = 1,
                    }
                }
            }
            };

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.GetPendingPolicyRequirements(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(testRequirementSummaries);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.GetPendingPolicyRequirements();
            var okResult = actionResult as OkObjectResult;

            // Assert
            Assert.IsType<RequirementSummariesResponse>(okResult.Value);
        }

        [Fact]
        public async Task GetPolicyStatusesAsync_ShouldReturn_ProblemDetails_ErrorMessage()
        {
            // Arrange
            var testAgentId = "4580";
            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.GetPolicyStatusCounts(
                It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.GetPolicyStatusCounts();
            var result = actionResult as ObjectResult;

            var details = result.Value as ProblemDetails;

            // Assert
            Assert.Equal("An unexpected error occured.", details.Detail);
            Assert.Equal("InternalServerError", details.Title);
            Assert.Equal(500, details.Status);
        }

        [Fact]
        public async Task GetPolicyStatusesAsync_ShouldReturn_PolicyStatusCounts()
        {
            // Arrange
            var testAgentId = "4580";

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.GetPolicyStatusCounts(
                It.IsAny<string>()))
                .ReturnsAsync(() => new PolicyStatusCountsResponse());

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.GetPolicyStatusCounts();
            var okResult = actionResult as OkObjectResult;

            // Assert
            Assert.IsType<PolicyStatusCountsResponse>(okResult.Value);
        }

        [Fact]
        public async Task DownloadPolicy_ShouldReturn_PolicyContents()
        {
            // Arrange
            var testAgentId = "4580";
            var policyNumber = "abc123456789";

            var fakePolicyPages = new Faker<FileResponse>("en")
                .CustomInstantiator(f => new FileResponse(f.Random.String(), MediaTypeNames.Application.Pdf))
                .RuleFor(x => x.FileData, f => f.Random.Bytes(8));

            var policyPages = fakePolicyPages.Generate();

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(true);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            mockDocumentVaultManager.Setup(manager => manager.GetPolicyPages(It.IsAny<string>()))
                .ReturnsAsync(policyPages);

            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var mockHttpContext = GetMockHttpContext(testAgentId);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.ExportPolicy(policyNumber);
            var okResult = actionResult as FileContentResult;

            // Assert
            Assert.IsType<FileContentResult>(okResult);
        }

        [Fact]
        public async Task DownloadPolicy_EmptyPolicy_ShouldReturn_BadRequest()
        {
            // Arrange
            var testAgentId = "4580";
            var policyNumber = string.Empty;

            var fakePolicyPages = new Faker<FileResponse>("en")
                .CustomInstantiator(f => new FileResponse(f.Random.String(), MediaTypeNames.Application.Pdf))
                .RuleFor(x => x.FileData, f => f.Random.Bytes(8));

            var policyPages = fakePolicyPages.Generate();

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(true);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            mockDocumentVaultManager.Setup(manager => manager.GetPolicyPages(It.IsAny<string>()))
                .ReturnsAsync(policyPages);

            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var mockHttpContext = GetMockHttpContext(testAgentId);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.ExportPolicy(policyNumber);
            var result = actionResult as ObjectResult;

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task DownloadPolicy_NoAccess_ShouldReturn_Forbidden()
        {
            // Arrange
            var testAgentId = "4580";
            var policyNumber = "abc123456789";

            var fakePolicyPages = new Faker<FileResponse>("en")
                .CustomInstantiator(f => new FileResponse(f.Random.String(), MediaTypeNames.Application.Pdf))
                .RuleFor(x => x.FileData, f => f.Random.Bytes(8));

            var policyPages = fakePolicyPages.Generate();

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(false);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            mockDocumentVaultManager.Setup(manager => manager.GetPolicyPages(It.IsAny<string>()))
                .ReturnsAsync(policyPages);

            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var mockHttpContext = GetMockHttpContext(testAgentId);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.ExportPolicy(policyNumber);
            var forbidResult = actionResult as ForbidResult;

            // Assert
            Assert.IsType<ForbidResult>(forbidResult);
        }

        [Fact]
        public async Task DownloadApplication_ShouldReturn_Application()
        {
            // Arrange
            var testAgentId = "4580";
            var policyNumber = "abc123456789";
            var fakeFileName = "application-" + policyNumber + ".pdf";
            var fakeFileType = MediaTypeNames.Application.Pdf;

            var fakeApplication = new Faker<FileResponse>("en")
                .CustomInstantiator(f => new FileResponse(fakeFileName, fakeFileType))
                .RuleFor(x => x.FileData, f => f.Random.Bytes(8));

            var application = fakeApplication.Generate();

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(true);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);

            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);
            mockExecute360DocumentManager.Setup(manager => manager.GetApplication(policyNumber))
                .ReturnsAsync(application);

            var mockHttpContext = GetMockHttpContext(testAgentId);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.ExportApplication(policyNumber);
            var okResult = actionResult as FileContentResult;

            // Assert
            Assert.IsType<FileContentResult>(okResult);
            Assert.Equal("application-abc123456789.pdf", okResult.FileDownloadName);
            Assert.Equal("application/pdf", okResult.ContentType);
        }

        [Fact]
        public async Task DownloadApplication_EmptyPolicy_ShouldReturn_BadRequest()
        {
            // Arrange
            var testAgentId = "4580";
            var policyNumber = string.Empty;
            var fakeFileName = "application-" + policyNumber + ".pdf";
            var fakeFileType = MediaTypeNames.Application.Pdf;

            var fakeApplication = new Faker<FileResponse>("en")
                .CustomInstantiator(f => new FileResponse(fakeFileName, fakeFileType))
                .RuleFor(x => x.FileData, f => f.Random.Bytes(8));

            var application = fakeApplication.Generate();

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(true);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);

            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);
            mockExecute360DocumentManager.Setup(manager => manager.GetApplication(policyNumber))
                .ReturnsAsync(application);

            var mockHttpContext = GetMockHttpContext(testAgentId);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.ExportApplication(policyNumber);
            var result = actionResult as ObjectResult;

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task DownloadApplication_NoAccess_ShouldReturn_Forbidden()
        {
            // Arrange
            var testAgentId = "4580";
            var policyNumber = "abc123456789";
            var fakeFileName = "application-" + policyNumber + ".pdf";
            var fakeFileType = MediaTypeNames.Application.Pdf;

            var fakeApplication = new Faker<FileResponse>("en")
                .CustomInstantiator(f => new FileResponse(fakeFileName, fakeFileType))
                .RuleFor(x => x.FileData, f => f.Random.Bytes(8));

            var application = fakeApplication.Generate();

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(false);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);

            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);
            mockExecute360DocumentManager.Setup(manager => manager.GetApplication(policyNumber))
                .ReturnsAsync(application);

            var mockHttpContext = GetMockHttpContext(testAgentId);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.ExportApplication(policyNumber);
            var forbidResult = actionResult as ForbidResult;

            // Assert
            Assert.IsType<ForbidResult>(forbidResult);
        }

        [Fact]
        public async Task DownloadApplication_NotFoundPolicyNumber_ShouldReturn_ErrorMessage()
        {
            // Arrange
            var testAgentId = "4580";
            var policyNumber = "abc123456789";

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(true);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);

            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);
            mockExecute360DocumentManager.Setup(manager => manager.GetApplication(policyNumber))
                .ReturnsAsync((FileResponse)null);

            var mockHttpContext = GetMockHttpContext(testAgentId);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.ExportApplication(policyNumber);
            var result = actionResult as ObjectResult;

            var details = result.Value as ProblemDetails;

            // Assert
            Assert.Equal("An unexpected error occured.", details.Detail);
            Assert.Equal("InternalServerError", details.Title);
            Assert.Equal(500, details.Status);
        }

        [Fact]
        public async Task DownloadApplication_ForbiddenApplication_ShouldReturn_ErrorMessage()
        {
            // Arrange
            var testAgentId = "4580";
            var policyNumber = "abc123456789";

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.CheckAgentAccessToPolicy(
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync(true);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);

            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);
            mockExecute360DocumentManager
                .Setup(manager => manager.GetApplication(policyNumber))
                .ReturnsAsync((FileResponse)null);

            var mockHttpContext = GetMockHttpContext(testAgentId);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.ExportApplication(policyNumber);
            var result = actionResult as ObjectResult;

            var details = result.Value as ProblemDetails;

            // Assert
            Assert.Equal("An unexpected error occured.", details.Detail);
            Assert.Equal("InternalServerError", details.Title);
            Assert.Equal(500, details.Status);
        }

        [Fact]
        public async Task DownloadPoliciesIntoExcelDocument_PendingStatus_ShouldReturn_ExcelDocument()
        {
            // Arrange
            var testAgentId = "4580";
            var testStatus = Status.Pending;
            var fakeDocument = new Faker().Random.Bytes(10);

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"{testAgentId}_{testStatus}Policies_{DateTime.Now:yyyy'-'MM'-'dd}";
            var fakeDocumentResponse = new FileResponse("fakeDocument", fileType)
            {
                FileData = fakeDocument.ToArray()
            };

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.GetPolicySummariesAsExcelDocument(
                testAgentId,
                testStatus,
                It.IsAny<string>()))
                .ReturnsAsync(fakeDocumentResponse);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.ExportPoliciesByStatus(testStatus);
            var okResult = actionResult as FileContentResult;

            // Assert
            Assert.IsType<FileContentResult>(okResult);
        }

        [Fact]
        public async Task DownloadPoliciesIntoExcelDocument_ActiveStatus_ShouldReturn_ExcelDocument()
        {
            // Arrange
            var testAgentId = "4580";
            var testStatus = Status.Active;
            var fakeDocument = new Faker().Random.Bytes(10);

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"{testAgentId}_{testStatus}Policies_{DateTime.Now:yyyy'-'MM'-'dd}";
            var fakeDocumentResponse = new FileResponse("fakeDocument", fileType)
            {
                FileData = fakeDocument.ToArray()
            };

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.GetPolicySummariesAsExcelDocument(
                testAgentId,
                testStatus,
                It.IsAny<string>()))
                .ReturnsAsync(fakeDocumentResponse);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.ExportPoliciesByStatus(testStatus);
            var okResult = actionResult as FileContentResult;

            // Assert
            Assert.IsType<FileContentResult>(okResult);
        }

        [Fact]
        public async Task DownloadPoliciesIntoExcelDocument_TerminatedStatus_ShouldReturn_ExcelDocument()
        {
            // Arrange
            var testAgentId = "4580";
            var testStatus = Status.Terminated;
            var fakeDocument = new Faker().Random.Bytes(10);

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = $"{testAgentId}_{testStatus}Policies_{DateTime.Now:yyyy'-'MM'-'dd}";
            var fakeDocumentResponse = new FileResponse("fakeDocument", fileType)
            {
                FileData = fakeDocument.ToArray()
            };

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.GetPolicySummariesAsExcelDocument(
                testAgentId,
                testStatus,
                It.IsAny<string>()))
                .ReturnsAsync(fakeDocumentResponse);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.ExportPoliciesByStatus(testStatus);
            var okResult = actionResult as FileContentResult;

            // Assert
            Assert.IsType<FileContentResult>(okResult);
        }

        [Fact]
        public async Task DownloadPoliciesIntoExcelDocument_NoPolicies_ShouldReturn_ErrorMessage()
        {
            // Arrange
            var testAgentId = "4580";
            var testStatus = Status.Active;

            var mockLogger = new Mock<ILogger<PolicyInfoController>>();
            var mockHttpRequestMessageValuesProvider = GetMockHttpRequestMessageValuesProvider();

            var mockPolicyInfoManager = new Mock<IPolicyInfoManager>(MockBehavior.Strict);
            mockPolicyInfoManager.Setup(manager => manager.GetPolicySummariesAsExcelDocument(
                testAgentId,
                testStatus,
                It.IsAny<string>()))
                .ReturnsAsync((FileResponse)null);

            var mockConfigurationManager = new Mock<IConfigurationManager>(MockBehavior.Strict);
            mockConfigurationManager.Setup(manager => manager.AzureAdIssuer).Returns("Issuer");
            mockConfigurationManager.Setup(manager => manager.PingOneIssuer).Returns("Ping");

            var mockHttpContext = GetMockHttpContext(testAgentId);
            var mockDocumentVaultManager = new Mock<IDocumentVaultManager>(MockBehavior.Strict);
            var mockExecute360DocumentManager = new Mock<IExecute360DocumentManager>(MockBehavior.Strict);

            var infoController = GetInfoController(
                mockHttpContext,
                mockLogger,
                mockHttpRequestMessageValuesProvider,
                mockPolicyInfoManager,
                mockDocumentVaultManager,
                mockExecute360DocumentManager,
                mockConfigurationManager);

            // Act
            var actionResult = await infoController.ExportPoliciesByStatus(testStatus);
            var result = actionResult as ObjectResult;

            var details = result.Value as ProblemDetails;

            // Assert
            Assert.Equal("An unexpected error occured.", details.Detail);
            Assert.Equal("InternalServerError", details.Title);
            Assert.Equal(500, details.Status);
        }

        private HttpContext GetMockHttpContext(string agentId)
        {
            var claims = new List<Claim>
            {
                new Claim("agentId", agentId, string.Empty, "Ping"),
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var context = new DefaultHttpContext
            {
                User = user,
                Items = new Dictionary<object, object>
                {
                    { "AgentId", agentId },
                    { "IsSubaccount", false }
                }
            };

            context.Request.Headers.Append("Guid", Guid.NewGuid().ToString());

            return context;
        }

        private Mock<IHttpRequestMessageValuesProvider> GetMockHttpRequestMessageValuesProvider()
        {
            var mockHttpRequestMessageValuesProvider = new Mock<IHttpRequestMessageValuesProvider>(MockBehavior.Strict);
            mockHttpRequestMessageValuesProvider
                .Setup(httpRequestMessageValuesProvider => httpRequestMessageValuesProvider.ExtractGuid(
                    It.IsAny<IHeaderDictionary>()))
                .Returns(Guid.NewGuid());

            return mockHttpRequestMessageValuesProvider;
        }

        private PolicyInfoController GetInfoController(
            HttpContext httpContext,
            Mock<ILogger<PolicyInfoController>> mockLogger,
            Mock<IHttpRequestMessageValuesProvider> mockHttpRequestMessageValuesProvider,
            Mock<IPolicyInfoManager> mockPolicyInfoManager,
            Mock<IDocumentVaultManager> mockDocumentVaultManager,
            Mock<IExecute360DocumentManager> mockExecute360DocumentManager,
            Mock<IConfigurationManager> mockConfigurationManager)
        {
            var infoController = new PolicyInfoController(
                mockLogger.Object,
                mockHttpRequestMessageValuesProvider.Object,
                mockPolicyInfoManager.Object,
                mockDocumentVaultManager.Object,
                mockExecute360DocumentManager.Object,
                mockConfigurationManager.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            return infoController;
        }
    }
}
