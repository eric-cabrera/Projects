namespace Assurity.AgentPortal.Managers.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.AgentPortal.Accessors.Agent;
    using Assurity.AgentPortal.Accessors.Integration;
    using Assurity.AgentPortal.Contracts.AgentContracts;
    using Assurity.AgentPortal.Contracts.Enums;
    using Assurity.AgentPortal.Engines;
    using Assurity.AgentPortal.Managers.AgentHierarchy;
    using Assurity.AgentPortal.Utilities.Configs;
    using Assurity.AgentPortal.Utilities.Encryption;
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;
    using AgentAPI = Assurity.Agent.Contracts.ActiveHierarchy;

    [ExcludeFromCodeCoverage]
    public class AgentManagerTests
    {
        [Fact]
        public async Task GetAgentContractsAsync_Success()
        {
            // Arrange
            var agentId = "ABC1";

            // Ensure mock contracts data structure matches expected format
            var contractsResponse = new List<Assurity.Agent.Contracts.AgentContract>
                {
                    new Assurity.Agent.Contracts.AgentContract
                    {
                        AgentNumber = "672M",
                        MarketCode = "AGTDTC",
                        Level = "90"
                    },
                    new Assurity.Agent.Contracts.AgentContract
                    {
                        AgentNumber = "672M",
                        MarketCode = "IG",
                        Level = "01"
                    },
                    new Assurity.Agent.Contracts.AgentContract
                    {
                        AgentNumber = "672M",
                        MarketCode = "WSR11",
                        Level = "90"
                    },
                    new Assurity.Agent.Contracts.AgentContract
                    {
                        AgentNumber = "672M",
                        MarketCode = "WSR12",
                        Level = "90"
                    }
                };

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            mockAgentApiAccessor.Setup(accessor => accessor.GetAgentContracts(
                     It.IsAny<string>(),
                     false,
                     Assurity.Agent.Contracts.MarketCodeFilters.AgentCenter,
                     It.IsAny<CancellationToken>(),
                     It.IsAny<string>()))
                 .ReturnsAsync(contractsResponse)
                 .Verifiable();

            mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "ABC1", "DEF2" });

            var mockDataStoreAccessor = new Mock<IIntegrationAccessor>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            var mockEncryption = new Mock<IEncryption>();

            var mockFileExportEngine = new Mock<IFileExportEngine>();
            var mockMapper = new Mock<IMapper>();
            bool includeAssociatedAgentNumbers = false;
            MarketCodeFilters marketCodeFilter = MarketCodeFilters.AgentCenter;

            var listBillManager = new AgentHierarchyManager(
                mockAgentApiAccessor.Object,
                mockDataStoreAccessor.Object,
                mockConfigurationManager.Object,
                mockEncryption.Object,
                mockMapper.Object,
                mockFileExportEngine.Object);

            // Act
            var response = await listBillManager.GetAgentContracts(agentId, includeAssociatedAgentNumbers, marketCodeFilter, CancellationToken.None);

            Assurity.Agent.Contracts.MarketCodeFilters mappedMarketCodeFilter =
             Enum.TryParse(marketCodeFilter.ToString(), out Assurity.Agent.Contracts.MarketCodeFilters result)
             ? result
             : Assurity.Agent.Contracts.MarketCodeFilters.None;

            // Assert
            Assert.NotNull(response);
            mockAgentApiAccessor.Verify(
                accessor => accessor.GetAgentContracts(
                agentId,
                includeAssociatedAgentNumbers,
                mappedMarketCodeFilter,
                CancellationToken.None,
                null),
                Times.Once);
        }

        [Fact]
        public async Task GetAgentContractsAsync_NullResponse_ShouldReturn_Null()
        {
            // Arrange
            var agentId = "ABC1";

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            mockAgentApiAccessor.Setup(accessor => accessor.GetAgentContracts(
                    It.IsAny<string>(),
                    false,
                    Assurity.Agent.Contracts.MarketCodeFilters.AgentCenter,
                    It.IsAny<CancellationToken>(),
                    It.IsAny<string>()))
                .ReturnsAsync(() => null);
            mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "ABC1", "DEF2" });
            var mockFileExportEngine = new Mock<IFileExportEngine>();
            var mockMapper = new Mock<IMapper>();
            var mockDataStoreAccessor = new Mock<IIntegrationAccessor>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            var mockEncryption = new Mock<IEncryption>();

            var listBillManager = new AgentHierarchyManager(
                mockAgentApiAccessor.Object,
                mockDataStoreAccessor.Object,
                mockConfigurationManager.Object,
                mockEncryption.Object,
                mockMapper.Object,
                mockFileExportEngine.Object);

            // Act
            bool includeAssociatedAgentNumbers = false;
            MarketCodeFilters marketCodeFilter = MarketCodeFilters.AgentCenter;

            // Act
            var response = await listBillManager.GetAgentContracts(agentId, includeAssociatedAgentNumbers, marketCodeFilter, CancellationToken.None);

            // Assert
            Assert.Null(response);
        }

#pragma warning disable SA1204 // Static elements should appear before instance elements
        // Suppressing this warning so that the arguments for a test theory can appear immediately before the test
        public static IEnumerable<object[]> GetMarketCodeInformation_Success_Data()
#pragma warning restore SA1204 // Static elements should appear before instance elements
        {
            yield return new object[] { "ABCD", new List<string>(), new List<string>(), false, false };
            yield return new object[] { "ABCD", new List<string> { "ABCD    " }, new List<string>(), true, false };
            yield return new object[] { "ABCD", new List<string>(), new List<string> { "ABCD    " }, false, true };
            yield return new object[] { "ABCD", new List<string> { "ABCD    " }, new List<string> { "ABCD    " }, true, true };
        }

        [Theory]
        [MemberData(nameof(GetMarketCodeInformation_Success_Data))]
        public async Task GetMarketCodeInformation_Success(
            string marketCode,
            List<string> reverseHierarchyMarketCodes,
            List<string> nyMarketCodes,
            bool expectedReverseHierarchyMarketCode,
            bool expectedNYMarketCode)
        {
            // Arrange
            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            var mockFileExportEngine = new Mock<IFileExportEngine>();
            var mockDataStoreAccessor = new Mock<IIntegrationAccessor>();
            mockDataStoreAccessor.Setup(accessor => accessor.GetReverseHierarchyMarketCodes())
                .ReturnsAsync(reverseHierarchyMarketCodes);
            mockDataStoreAccessor.Setup(accessor => accessor.GetNewYorkMarketCodes())
                .ReturnsAsync(nyMarketCodes);

            var mockConfigurationManager = new Mock<IConfigurationManager>();
            var mockMapper = new Mock<IMapper>();

            var mockEncryption = new Mock<IEncryption>();

            var agentManager = new AgentHierarchyManager(
                mockAgentApiAccessor.Object,
                mockDataStoreAccessor.Object,
                mockConfigurationManager.Object,
                mockEncryption.Object,
                mockMapper.Object,
                mockFileExportEngine.Object);

            // Act
            var response = await agentManager.GetMarketCodeInformation(marketCode);

            // Assert
            Assert.Equal(1, mockDataStoreAccessor.Invocations.Count(i => i.Method.Name == "GetReverseHierarchyMarketCodes"));
            Assert.Equal(1, mockDataStoreAccessor.Invocations.Count(i => i.Method.Name == "GetNewYorkMarketCodes"));
            Assert.Equal(expectedReverseHierarchyMarketCode, response.IsReverseHirearchyMarketCode);
            Assert.Equal(expectedNYMarketCode, response.IsNewYorkMarketCode);
        }

        [Fact]
        public async Task GetMarketCodeInformation_ThrowsOnNull()
        {
            // Arrange
            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();

            var exMessage = "DB QUERY FAILED";
            var ex = new Exception(exMessage);

            var mockDataStoreAccessor = new Mock<IIntegrationAccessor>();
            mockDataStoreAccessor.Setup(accessor => accessor.GetReverseHierarchyMarketCodes())
                .Throws(ex);
            mockDataStoreAccessor.Setup(accessor => accessor.GetNewYorkMarketCodes())
                .Throws(ex);
            var mockMapper = new Mock<IMapper>();
            var mockFileExportEngine = new Mock<IFileExportEngine>();
            var mockEncryption = new Mock<IEncryption>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();

            var agentManager = new AgentHierarchyManager(
                mockAgentApiAccessor.Object,
                mockDataStoreAccessor.Object,
                mockConfigurationManager.Object,
                mockEncryption.Object,
                mockMapper.Object,
                mockFileExportEngine.Object);

            // Act & Assert
            var result = Assert.ThrowsAsync<Exception>(async () => await agentManager.GetMarketCodeInformation("ABCD"));
            Assert.Equal(exMessage, result?.Result.Message);
        }

        [Fact]
        public async Task GetVertaforeInformation_Succeeds()
        {
            // Arrange
            var agentId = "ABC1";
            var agentUsername = "username";
            var agentEmail = "email@assurity.com";

            var groupsResponse = new List<Assurity.Agent.Contracts.AgentContract>();
            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            mockAgentApiAccessor.Setup(accessor => accessor.GetAgentVertaforeContracts(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(groupsResponse)
                .Verifiable();
            var mockDataStoreAccessor = new Mock<IIntegrationAccessor>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            var mockEncryption = new Mock<IEncryption>();
            var mockFileExportEngine = new Mock<IFileExportEngine>();
            var mockMapper = new Mock<IMapper>();

            var listBillManager = new AgentHierarchyManager(
                mockAgentApiAccessor.Object,
                mockDataStoreAccessor.Object,
                mockConfigurationManager.Object,
                mockEncryption.Object,
                mockMapper.Object,
                mockFileExportEngine.Object);

            // Act
            var response = await listBillManager.GetVertaforeInformation(
                agentId,
                agentUsername,
                agentEmail,
                CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            mockAgentApiAccessor.Verify(
                accessor => accessor.GetAgentVertaforeContracts(
                agentId,
                CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task GetVertaforeInformation_NullResponse_Returns_Null()
        {
            // Arrange
            var agentId = "ABC1";
            var agentUsername = "username";
            var agentEmail = "email@assurity.com";

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            mockAgentApiAccessor.Setup(accessor => accessor.GetAgentVertaforeContracts(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => null);
            var mockDataStoreAccessor = new Mock<IIntegrationAccessor>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            var mockEncryption = new Mock<IEncryption>();
            var mockFileExportEngine = new Mock<IFileExportEngine>();
            var mockMapper = new Mock<IMapper>();
            var listBillManager = new AgentHierarchyManager(
                mockAgentApiAccessor.Object,
                mockDataStoreAccessor.Object,
                mockConfigurationManager.Object,
                mockEncryption.Object,
                mockMapper.Object,
                mockFileExportEngine.Object);

            // Act
            var response = await listBillManager.GetVertaforeInformation(
                agentId,
                agentUsername,
                agentEmail,
                CancellationToken.None);

            // Assert
            Assert.Null(response);
        }

        [Fact]
        public async Task GetViewAsDropdownOptions_Success()
        {
            // Arrange
            var agentId = "ABC1";

            var agentContractsResponse = new List<Assurity.Agent.Contracts.AgentContract>
        {
            new Assurity.Agent.Contracts.AgentContract
            {
                AgentNumber = "ABC1",
                MarketCode = "MXQ2035",
                CompanyCode = "C001",
                Level = "1"
            },
            new Assurity.Agent.Contracts.AgentContract
            {
                AgentNumber = "ABC1",
                MarketCode = "MXQ2036",
                CompanyCode = "C002",
                Level = "2"
            }
        };

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            mockAgentApiAccessor.Setup(accessor => accessor.GetAgentContracts(
                   It.IsAny<string>(),
                   It.IsAny<bool>(),
                   It.IsAny<Assurity.Agent.Contracts.MarketCodeFilters>(),
                   It.IsAny<CancellationToken>(),
                   It.IsAny<string>()))
               .ReturnsAsync(agentContractsResponse);

            mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(agentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "ABC1", "DEF2" });

            var mockFileExportEngine = new Mock<IFileExportEngine>();
            var mockEncryption = new Mock<IEncryption>();
            var mockMapper = new Mock<IMapper>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            var mockDataStoreAccessor = new Mock<IIntegrationAccessor>();

            var agentManager = new AgentHierarchyManager(
                mockAgentApiAccessor.Object,
                mockDataStoreAccessor.Object,
                mockConfigurationManager.Object,
                mockEncryption.Object,
                mockMapper.Object,
                mockFileExportEngine.Object);

            // Act
            var dropdownOptions = await agentManager.GetViewAsDropdownOptions(agentId, true, MarketCodeFilters.AgentCenter, CancellationToken.None);

            // Assert
            Assert.NotNull(dropdownOptions);
            Assert.Equal(2, dropdownOptions.Count);
            Assert.Equal("ABC1-MXQ2035-1", dropdownOptions[0].DisplayValue);
            Assert.Equal("ABC1", dropdownOptions[0].AgentNumber);
            Assert.Equal("MXQ2035", dropdownOptions[0].MarketCode);
            Assert.Equal("C001", dropdownOptions[0].CompanyCode);
            Assert.Equal("1", dropdownOptions[0].AgentLevel);

            Assert.Equal("ABC1-MXQ2036-2", dropdownOptions[1].DisplayValue);
            Assert.Equal("ABC1", dropdownOptions[1].AgentNumber);
            Assert.Equal("MXQ2036", dropdownOptions[1].MarketCode);
            Assert.Equal("C002", dropdownOptions[1].CompanyCode);
            Assert.Equal("2", dropdownOptions[1].AgentLevel);
        }

        [Fact]
        public async Task GetViewAsDropdownOptions_EmptyResponse_ShouldReturn_Null()
        {
            // Arrange
            var agentId = "ABC1";

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            mockAgentApiAccessor
                .Setup(accessor => accessor.GetAgentContracts(
                   It.IsAny<string>(),
                   It.IsAny<bool>(),
                   It.IsAny<Assurity.Agent.Contracts.MarketCodeFilters>(),
                   It.IsAny<CancellationToken>(),
                   It.IsAny<string>()))
               .ReturnsAsync((List<Assurity.Agent.Contracts.AgentContract>)null);

            mockAgentApiAccessor
                .Setup(accessor => accessor.GetAdditionalAgentIds(agentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "ABC1", "DEF2" });

            var mockFileExportEngine = new Mock<IFileExportEngine>();
            var mockMapper = new Mock<IMapper>();
            var mockDataStoreAccessor = new Mock<IIntegrationAccessor>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();
            var mockEncryption = new Mock<IEncryption>();

            var agentManager = new AgentHierarchyManager(
                mockAgentApiAccessor.Object,
                mockDataStoreAccessor.Object,
                mockConfigurationManager.Object,
                mockEncryption.Object,
                mockMapper.Object,
                mockFileExportEngine.Object);

            // Act
            var dropdownOptions = await agentManager.GetViewAsDropdownOptions(agentId, true, MarketCodeFilters.AgentCenter, CancellationToken.None);

            // Assert
            Assert.Null(dropdownOptions);
        }

        [Fact]
        public async Task GetViewAsDropdownOptions_NullResponse_ShouldReturn_Null()
        {
            // Arrange
            var agentId = "ABC1";

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            mockAgentApiAccessor
                .Setup(accessor => accessor.GetAgentContracts(
                   It.IsAny<string>(),
                   It.IsAny<bool>(),
                   It.IsAny<Assurity.Agent.Contracts.MarketCodeFilters>(),
                   It.IsAny<CancellationToken>(),
                   It.IsAny<string>()))
               .ReturnsAsync((List<Assurity.Agent.Contracts.AgentContract>)null);

            mockAgentApiAccessor
                .Setup(accessor => accessor.GetAdditionalAgentIds(agentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "ABC1", "DEF2" });

            var mockFileExportEngine = new Mock<IFileExportEngine>();
            var mockEncryption = new Mock<IEncryption>();
            var mockMapper = new Mock<IMapper>();
            var mockConfigurationManager = new Mock<IConfigurationManager>();

            var mockDataStoreAccessor = new Mock<IIntegrationAccessor>();

            var agentManager = new AgentHierarchyManager(
                mockAgentApiAccessor.Object,
                mockDataStoreAccessor.Object,
                mockConfigurationManager.Object,
                mockEncryption.Object,
                mockMapper.Object,
                mockFileExportEngine.Object);

            // Act
            var dropdownOptions = await agentManager.GetViewAsDropdownOptions(agentId, true, MarketCodeFilters.AgentCenter, CancellationToken.None);

            // Assert
            Assert.Null(dropdownOptions);
        }

        [Fact]
        public async Task GetAgentHierarchyAsync_Success()
        {
            // Arrange
            var agentNumber = "A12345";
            var marketCode = "Market1";
            var agentLevel = "Level1";
            var companyCode = "Company1";
            var contractStatus = ContractStatus.Active;
            var includeAgentInformation = true;
            var includePendingRequirements = false;
            var filterAgentsWithoutPendingRequirements = true;
            var cancellationToken = CancellationToken.None;

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();

            var apiResponse = new AgentAPI.ActiveHierarchyResponse
            {
                Hierarchy = new AgentAPI.AgentHierarchy
                {
                    TotalActiveAgents = 100,
                    TotalJitAgents = 50,
                    TotalPendingAgents = 25
                },
                Filters = new AgentAPI.ActiveHierarchyFilters
                {
                    AgentNames = new Dictionary<string, string>
            {
                { "A12345", "John Doe" }
            }
                }
            };

            mockAgentApiAccessor
                .Setup(x => x.GetAgentHierarchy(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<ContractStatus?>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await mockAgentApiAccessor.Object.GetAgentHierarchy(
                agentNumber,
                marketCode,
                agentLevel,
                companyCode,
                contractStatus,
                includeAgentInformation,
                includePendingRequirements,
                filterAgentsWithoutPendingRequirements,
                cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Hierarchy);
            Assert.Equal(100, result.Hierarchy.TotalActiveAgents);
            Assert.Equal(50, result.Hierarchy.TotalJitAgents);
            Assert.Equal(25, result.Hierarchy.TotalPendingAgents);
            Assert.Single(result.Filters.AgentNames);
            Assert.Equal("John Doe", result.Filters.AgentNames["A12345"]);
        }

        [Fact]
        public async Task GetActiveAppointmentsAsync_Success()
        {
            // Arrange
            var agentNumber = "A12345";
            var marketCode = "Market1";
            var agentLevel = "Level1";
            var companyCode = "Company1";
            var state = Assurity.AgentPortal.Contracts.Enums.State.PA;
            var page = 1;
            var pageSize = 10;
            var cancellationToken = CancellationToken.None;

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            var mockFileIMapper = new Mock<IMapper>();

            var apiResponse = new AgentAPI.AgentAppointmentResponse
            {
                Appointments = new List<Assurity.Agent.Contracts.AgentAppointment>
                 {
                    new Assurity.Agent.Contracts.AgentAppointment
                    {
                        AgentNumber = agentNumber,
                        CompanyCode = companyCode,
                        Name = "John Doe",
                        IsResident = true,
                        StateAbbreviation = "CA"
                    }
                 },
                AppointedStates = new List<string> { "CA", "TX" },
                Page = 1,
                PageSize = 10,
                TotalPages = 1
            };

            mockAgentApiAccessor
                .Setup(x => x.GetActiveAppointments(agentNumber, marketCode, agentLevel, companyCode, state, page, pageSize, cancellationToken))
                .ReturnsAsync(apiResponse);

            mockFileIMapper
                .Setup(m => m.Map<AgentAPI.AgentAppointmentResponse>(It.IsAny<object>()))
                .Returns(apiResponse);

            // Act
            var result = await mockAgentApiAccessor.Object.GetActiveAppointments(
                agentNumber,
                marketCode,
                agentLevel,
                companyCode,
                state,
                page,
                pageSize,
                cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Appointments);
            Assert.Equal("John Doe", result.Appointments[0].Name);
        }

        [Fact]
        public async Task GetActiveAppointmentsAsync_EmptyResponse_ReturnsEmptyAppointments()
        {
            // Arrange
            var agentNumber = "A12345";
            var marketCode = "Market1";
            var agentLevel = "Level1";
            var companyCode = "Company1";
            var state = Assurity.AgentPortal.Contracts.Enums.State.PA;
            var page = 1;
            var pageSize = 10;
            var cancellationToken = CancellationToken.None;

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            var mockFileIMapper = new Mock<IMapper>();

            var apiResponse = new AgentAPI.AgentAppointmentResponse
            {
                Appointments = new List<Assurity.Agent.Contracts.AgentAppointment>(),
                AppointedStates = new List<string> { "CA", "TX" },
                Page = 1,
                PageSize = 10,
                TotalPages = 1
            };

            mockAgentApiAccessor
                .Setup(x => x.GetActiveAppointments(agentNumber, marketCode, agentLevel, companyCode, state, page, pageSize, cancellationToken))
                .ReturnsAsync(apiResponse);

            mockFileIMapper
                .Setup(m => m.Map<AgentAPI.AgentAppointmentResponse>(It.IsAny<object>()))
                .Returns(apiResponse);

            // Act
            var result = await mockAgentApiAccessor.Object.GetActiveAppointments(
                agentNumber,
                marketCode,
                agentLevel,
                companyCode,
                state,
                page,
                pageSize,
                cancellationToken);

            Assert.NotNull(result);
            Assert.Empty(result.Appointments);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
        }
    }
}