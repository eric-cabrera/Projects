namespace Assurity.AgentPortal.Managers.Tests
{
    using System.Diagnostics.CodeAnalysis;
    using Assurity.AgentPortal.Accessors.Agent;
    using Assurity.AgentPortal.Accessors.GroupInventory;
    using Assurity.AgentPortal.Accessors.MongoDb.Contracts;
    using Assurity.AgentPortal.Accessors.PolicyInfo;
    using Assurity.AgentPortal.Contracts.Enums;
    using Assurity.AgentPortal.Contracts.GroupInventory.Request;
    using Assurity.AgentPortal.Contracts.GroupInventory.Response;
    using Assurity.AgentPortal.Engines;
    using Assurity.AgentPortal.Managers.GroupInventory;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using AutoMapper;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class GroupInventoryManagerTests
    {
        [Fact]
        public async Task GetGroupSummary_Success_ReturnsGroupSummary()
        {
            // Arrange
            var agentNumber = "12345";
            var queryParams = new GroupSummaryQueryParameters
            {
                GroupNumber = "1001",
                GroupName = "Group A",
                GroupEffectiveStartDate = new DateTime(2022, 01, 01),
                GroupEffectiveEndDate = new DateTime(2022, 12, 31),
                GroupStatus = "Active",
                OrderBy = SummaryOrderBy.GroupStatus,
                SortDirection = SortDirection.DESC,
                Page = 1,
                PageSize = 10
            };

            var mockGroupSummaryResponse = new Assurity.Groups.Contracts.GroupSummaryResponse
            {
                GroupSummaries = new List<Assurity.Groups.Contracts.GroupSummary>
                {
                    new Assurity.Groups.Contracts.GroupSummary
                    {
                        GroupNumber = "1001",
                        GroupName = "Group A",
                        GroupStatus = "Active",
                        PolicyCount = 5,
                        GroupEffectiveDate = new DateTime(2022, 01, 01)
                    }
                },
                Filters = new Assurity.Groups.Contracts.GroupSummaryFilters()
            };

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(m => m.Map<Assurity.Groups.Contracts.GroupSummaryQueryParameters>(It.IsAny<GroupSummaryQueryParameters>()))
                .Returns(new Assurity.Groups.Contracts.GroupSummaryQueryParameters());

            mockMapper
                .Setup(m => m.Map<GroupSummaryResponse>(It.IsAny<Assurity.Groups.Contracts.GroupSummaryResponse>()))
                .Returns(new GroupSummaryResponse
                {
                    GroupSummaries = new List<GroupSummary>
                    {
                        new GroupSummary
                        {
                            Number = "1001",
                            Name = "Group A",
                            Status = "Active",
                            PolicyCount = 5,
                            GroupEffectiveDate = "01/01/20022"
                        }
                    },
                    Filters = new GroupSummaryFilters()
                });

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            mockAgentApiAccessor
                .Setup(api => api.GetAdditionalAgentIds(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "agent1", "agent2" });

            var mockGroupsApiAccessor = new Mock<IGroupInventoryApiAccessor>();
            var mockbenefitOptionsAccessor = new Mock<IBenefitOptionsAccessor>();

            mockGroupsApiAccessor
                .Setup(api => api.GetGroupSummary(agentNumber, It.IsAny<Assurity.Groups.Contracts.GroupSummaryQueryParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockGroupSummaryResponse);

            var mockLogger = new Mock<ILogger<GroupInventoryManager>>();
            var mockFileExportEngine = new Mock<IFileExportEngine>();

            var groupsManager = new GroupInventoryManager(mockGroupsApiAccessor.Object, mockbenefitOptionsAccessor.Object, mockAgentApiAccessor.Object, mockLogger.Object, mockMapper.Object, mockFileExportEngine.Object);

            // Act
            var result = await groupsManager.GetGroupSummary(agentNumber, queryParams, CancellationToken.None);

            Assert.NotNull(result);
            Assert.NotNull(result.GroupSummaries);
            Assert.Single(result.GroupSummaries);

            var groupSummary = result.GroupSummaries[0];
            Assert.Equal("1001", groupSummary.Number);
            Assert.Equal("Group A", groupSummary.Name);
            Assert.Equal("Active", groupSummary.Status);
            Assert.Equal("01/01/20022", groupSummary.GroupEffectiveDate);
            Assert.Equal(5, groupSummary.PolicyCount);
        }

        [Fact]
        public async Task GetGroupSummary_NoGroupsFound_ReturnsEmptyResponse()
        {
            // Arrange
            var agentNumber = "12345";
            var queryParams = new GroupSummaryQueryParameters
            {
                GroupNumber = "1001",
                GroupName = "Nonexistent Group",
                GroupEffectiveStartDate = new DateTime(2022, 01, 01),
                GroupEffectiveEndDate = new DateTime(2022, 12, 31),
                GroupStatus = "Active",
                OrderBy = SummaryOrderBy.GroupStatus,
                SortDirection = SortDirection.DESC,
                Page = 1,
                PageSize = 10
            };

            var mockGroupSummaryResponse = new Assurity.Groups.Contracts.GroupSummaryResponse
            {
                GroupSummaries = new List<Assurity.Groups.Contracts.GroupSummary>(),
                Filters = new Assurity.Groups.Contracts.GroupSummaryFilters()
            };

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(m => m.Map<Assurity.Groups.Contracts.GroupSummaryQueryParameters>(It.IsAny<GroupSummaryQueryParameters>()))
                .Returns(new Assurity.Groups.Contracts.GroupSummaryQueryParameters());

            mockMapper
                .Setup(m => m.Map<GroupSummaryResponse>(It.IsAny<Assurity.Groups.Contracts.GroupSummaryResponse>()))
                .Returns(new GroupSummaryResponse
                {
                    GroupSummaries = new List<GroupSummary>(),
                    Filters = new GroupSummaryFilters()
                });

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            mockAgentApiAccessor
                .Setup(api => api.GetAdditionalAgentIds(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "agent1", "agent2" });

            var mockGroupsApiAccessor = new Mock<IGroupInventoryApiAccessor>();
            mockGroupsApiAccessor
                .Setup(api => api.GetGroupSummary(agentNumber, It.IsAny<Assurity.Groups.Contracts.GroupSummaryQueryParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockGroupSummaryResponse);

            var mockLogger = new Mock<ILogger<GroupInventoryManager>>();
            var mockbenefitOptionsAccessor = new Mock<IBenefitOptionsAccessor>();
            var mockFileExportEngine = new Mock<IFileExportEngine>();

            var groupsManager = new GroupInventoryManager(mockGroupsApiAccessor.Object, mockbenefitOptionsAccessor.Object, mockAgentApiAccessor.Object, mockLogger.Object, mockMapper.Object, mockFileExportEngine.Object);

            // Act
            var result = await groupsManager.GetGroupSummary(agentNumber, queryParams, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetGroupSummary_AgentNotAuthorized_ThrowsException()
        {
            // Arrange
            var agentNumber = "12345";
            var queryParams = new GroupSummaryQueryParameters
            {
                GroupNumber = "1001",
                GroupName = "Group A",
                GroupEffectiveStartDate = new DateTime(2022, 01, 01),
                GroupEffectiveEndDate = new DateTime(2022, 12, 31),
                GroupStatus = "Active",
                OrderBy = SummaryOrderBy.GroupStatus,
                SortDirection = SortDirection.DESC,
                Page = 1,
                PageSize = 10,
                ViewAsAgentId = "67890"
            };

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            mockAgentApiAccessor
                .Setup(api => api.GetAdditionalAgentIds(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "agent1", "agent2" });

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(m => m.Map<Assurity.Groups.Contracts.GroupSummaryQueryParameters>(It.IsAny<GroupSummaryQueryParameters>()))
                .Returns(new Assurity.Groups.Contracts.GroupSummaryQueryParameters());

            var mockGroupsApiAccessor = new Mock<IGroupInventoryApiAccessor>();
            mockGroupsApiAccessor
                .Setup(api => api.GetGroupSummary(agentNumber, It.IsAny<Assurity.Groups.Contracts.GroupSummaryQueryParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Assurity.Groups.Contracts.GroupSummaryResponse
                {
                    GroupSummaries = new List<Assurity.Groups.Contracts.GroupSummary>
                    {
                new Assurity.Groups.Contracts.GroupSummary
                {
                    GroupNumber = "1001",
                    GroupName = "Group A",
                    GroupStatus = "Active",
                    PolicyCount = 5,
                    GroupEffectiveDate = new DateTime(2022, 01, 01)
                }
                    },
                    Filters = new Assurity.Groups.Contracts.GroupSummaryFilters()
                });

            var mockLogger = new Mock<ILogger<GroupInventoryManager>>();
            var mockbenefitOptionsAccessor = new Mock<IBenefitOptionsAccessor>();
            var mockFileExportEngine = new Mock<IFileExportEngine>();

            var groupsManager = new GroupInventoryManager(mockGroupsApiAccessor.Object, mockbenefitOptionsAccessor.Object, mockAgentApiAccessor.Object, mockLogger.Object, mockMapper.Object, mockFileExportEngine.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(async () => await groupsManager.GetGroupSummary(agentNumber, queryParams, CancellationToken.None));
        }

        [Fact]
        public async Task GetGroupDetail_Success_ReturnsGroupDetail()
        {
            // Arrange
            var agentNumber = "12345";
            var queryParams = new GroupDetailsQueryParameters
            {
                GroupNumber = "1001",
                PolicyNumber = "PN123",
                ProductDescription = "Product A",
                PolicyOwnerName = "John Doe",
                IssueStartDate = new DateTime(2022, 01, 01),
                IssueEndDate = new DateTime(2022, 12, 31),
                PolicyStatus = "Active",
                OrderBy = DetailsOrderBy.PolicyOwner,
                SortDirection = SortDirection.DESC,
                Page = 1,
                PageSize = 10
            };

            var mockGroupDetailResponse = new Assurity.Groups.Contracts.GroupDetailResponse
            {
                Group = new Assurity.Groups.Contracts.Group
                {
                    Name = "Group A",
                    Number = "1001",
                    Status = "Active",
                    EffectiveDate = new DateTime(2022, 01, 01),
                    ActivePolicyCount = 5
                },
                Policies = new List<Assurity.Groups.Contracts.Policy>
                    {
                        new Assurity.Groups.Contracts.Policy
                        {
                            Number = "PN123",
                            Status = "Active",
                            IssueDate = new DateTime(2022, 01, 01),
                            PaidToDate = new DateTime(2023, 01, 01),
                            AnnualPremium = 500.00m,
                            ModePremium = 50.00m,
                            PaymentType = "Monthly",
                            BillingMode = "Electronic",
                            ProductDescription = "Product A",
                            Benefits = new List<Assurity.Groups.Contracts.Benefit>()
                        }
                    }
            };

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(m => m.Map<Assurity.Groups.Contracts.GroupDetailsQueryParameters>(It.IsAny<GroupDetailsQueryParameters>()))
                .Returns(new Assurity.Groups.Contracts.GroupDetailsQueryParameters());

            mockMapper
                .Setup(m => m.Map<GroupDetailResponse>(It.IsAny<Assurity.Groups.Contracts.GroupDetailResponse>()))
                .Returns(new GroupDetailResponse
                {
                    Group = new Group
                    {
                        Name = "Group A",
                        Number = "1001",
                        Status = "Active",
                        EffectiveDate = "01/01/20022",
                        ActivePolicyCount = 5
                    },
                    Policies = new List<Policy>
                    {
                new Policy
                {
                    Number = "PN123",
                    Status = "Active",
                    IssueDate = "01/01/20022",
                    PaidToDate = "01/01/20022",
                    AnnualPremium = "$500.00",
                    ModePremium = "$50.00",
                    Mode = "Monthly Electronic",
                    ProductDescription = "Product A",
                    Benefits = new List<Benefit>()
                }
                    }
                });

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            mockAgentApiAccessor
                .Setup(api => api.GetAdditionalAgentIds(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "agent1", "agent2" });

            var mockGroupsApiAccessor = new Mock<IGroupInventoryApiAccessor>();
            mockGroupsApiAccessor
                .Setup(api => api.GetGroupDetail(agentNumber, queryParams.GroupNumber, It.IsAny<Assurity.Groups.Contracts.GroupDetailsQueryParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockGroupDetailResponse);

            var mockLogger = new Mock<ILogger<GroupInventoryManager>>();
            var mockbenefitOptionsAccessor = new Mock<IBenefitOptionsAccessor>();
            var mockFileExportEngine = new Mock<IFileExportEngine>();

            var groupsManager = new GroupInventoryManager(mockGroupsApiAccessor.Object, mockbenefitOptionsAccessor.Object, mockAgentApiAccessor.Object, mockLogger.Object, mockMapper.Object, mockFileExportEngine.Object);

            // Act
            var result = await groupsManager.GetGroupDetail(agentNumber, queryParams, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Group);
            Assert.Equal("1001", result.Group.Number);
            Assert.Equal("Group A", result.Group.Name);
            Assert.Equal("Active", result.Group.Status);
            Assert.Equal("01/01/20022", result.Group.EffectiveDate);
            Assert.Equal(5, result.Group.ActivePolicyCount);

            Assert.NotNull(result.Policies);
            Assert.Single(result.Policies);
            var policy = result.Policies[0];
            Assert.Equal("PN123", policy.Number);
            Assert.Equal("Active", policy.Status);
            Assert.Equal("01/01/20022", policy.IssueDate);
            Assert.Equal("01/01/20022", policy.PaidToDate);
            Assert.Equal("$500.00", policy.AnnualPremium);
            Assert.Equal("Product A", policy.ProductDescription);
        }

        [Fact]
        public async Task GetGroupDetail_NoGroupFound404WithResponseBody_ReturnsEmptyResponse()
        {
            // Arrange
            var agentNumber = "12345";
            var groupNumber = "1001";
            var queryParams = new GroupDetailsQueryParameters
            {
                GroupNumber = groupNumber,
                PolicyNumber = "PN999",
                ProductDescription = "Nonexistent Product",
                PolicyOwnerName = "Jane Doe",
                IssueStartDate = new DateTime(2022, 01, 01),
                IssueEndDate = new DateTime(2022, 12, 31),
                PolicyStatus = "Active",
                OrderBy = DetailsOrderBy.PolicyOwner,
                SortDirection = SortDirection.DESC,
                Page = 1,
                PageSize = 10
            };

            var mockGroupDetailResponse = new Assurity.Groups.Contracts.GroupDetailResponse
            {
                Group = null,
                Policies = new List<Assurity.Groups.Contracts.Policy>()
            };

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(m => m.Map<Assurity.Groups.Contracts.GroupDetailsQueryParameters>(It.IsAny<GroupDetailsQueryParameters>()))
                .Returns(new Assurity.Groups.Contracts.GroupDetailsQueryParameters());

            mockMapper
                .Setup(m => m.Map<GroupDetailResponse>(It.IsAny<Assurity.Groups.Contracts.GroupDetailResponse>()))
                .Returns(new GroupDetailResponse());

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            mockAgentApiAccessor
                .Setup(api => api.GetAdditionalAgentIds(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "agent1", "agent2" });

            var mockGroupsApiAccessor = new Mock<IGroupInventoryApiAccessor>();
            mockGroupsApiAccessor
                .Setup(api => api.GetGroupDetail(agentNumber, groupNumber, It.IsAny<Assurity.Groups.Contracts.GroupDetailsQueryParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockGroupDetailResponse);

            var mockLogger = new Mock<ILogger<GroupInventoryManager>>();
            var mockbenefitOptionsAccessor = new Mock<IBenefitOptionsAccessor>();
            var mockFileExportEngine = new Mock<IFileExportEngine>();

            var groupsManager = new GroupInventoryManager(mockGroupsApiAccessor.Object, mockbenefitOptionsAccessor.Object, mockAgentApiAccessor.Object, mockLogger.Object, mockMapper.Object, mockFileExportEngine.Object);

            // Act
            var result = await groupsManager.GetGroupDetail(agentNumber, queryParams, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Group);
            Assert.Null(result.Policies);
        }

        [Fact]
        public async Task FilterBenefitsWithHiddenCoverageOptions_HidesHiddenCoverageOptions()
        {
            // Arrange
            var agentNumber = "12345";
            var queryParams = new GroupDetailsQueryParameters
            {
                GroupNumber = "1001",
                PolicyNumber = "PN123",
                ProductDescription = "Product A",
                PolicyOwnerName = "John Doe",
                IssueStartDate = new DateTime(2022, 01, 01),
                IssueEndDate = new DateTime(2022, 12, 31),
                PolicyStatus = "Active",
                OrderBy = DetailsOrderBy.PolicyOwner,
                SortDirection = SortDirection.DESC,
                Page = 1,
                PageSize = 10
            };

            var mockGroupDetailResponse = new Assurity.Groups.Contracts.GroupDetailResponse
            {
                Group = new Assurity.Groups.Contracts.Group
                {
                    Name = "Group A",
                    Number = "1001",
                    Status = "Active",
                    EffectiveDate = new DateTime(2022, 01, 01),
                    ActivePolicyCount = 5
                },
                Policies = new List<Assurity.Groups.Contracts.Policy>
        {
            new Assurity.Groups.Contracts.Policy
            {
                Number = "PN123",
                Status = "Active",
                IssueDate = new DateTime(2022, 01, 01),
                PaidToDate = new DateTime(2023, 01, 01),
                AnnualPremium = 500.00m,
                ModePremium = 50.00m,
                PaymentType = "Monthly",
                BillingMode = "Electronic",
                ProductDescription = "Product A",
                Benefits = new List<Assurity.Groups.Contracts.Benefit>
                {
                    new Assurity.Groups.Contracts.Benefit
                    {
                        CoverageOptions = new List<Assurity.Groups.Contracts.CoverageOption>
                        {
                            new Assurity.Groups.Contracts.CoverageOption
                            {
                                Name = "AccidentalDeathAndDismemberment",
                                Value = "AccidentalDeathAndDismemberment"
                            },
                            new Assurity.Groups.Contracts.CoverageOption
                            {
                                Name = "Option2",
                                Value = "SomeOtherValue"
                            }
                        }
                    }
                }
            }
        }
            };

            var mockMapper = new Mock<IMapper>();
            mockMapper
                .Setup(m => m.Map<Assurity.Groups.Contracts.GroupDetailsQueryParameters>(It.IsAny<GroupDetailsQueryParameters>()))
                .Returns(new Assurity.Groups.Contracts.GroupDetailsQueryParameters());

            var mockGroupsApiAccessor = new Mock<IGroupInventoryApiAccessor>();
            mockGroupsApiAccessor
                .Setup(api => api.GetGroupDetail(agentNumber, queryParams.GroupNumber, It.IsAny<Assurity.Groups.Contracts.GroupDetailsQueryParameters>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockGroupDetailResponse);

            var mockBenefitOptionsAccessor = new Mock<IBenefitOptionsAccessor>();
            var hiddenCoverageOptions = new List<BenefitOptionsMapping>
            {
                new BenefitOptionsMapping
                {
                    Id = "some-id",
                    Category = BenefitOptionName.AccidentalDeathAndDismemberment,
                    Option = BenefitOptionValue.AccidentalDeathAndDismemberment,
                    HideBenefitOption = true
                }
            };
            mockBenefitOptionsAccessor
                .Setup(b => b.GetHiddenBenefitOptionsMappings())
                .ReturnsAsync(hiddenCoverageOptions);

            var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
            mockAgentApiAccessor
                .Setup(api => api.GetAdditionalAgentIds(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "agent1", "agent2" });

            var mockLogger = new Mock<ILogger<GroupInventoryManager>>();
            var mockFileExportEngine = new Mock<IFileExportEngine>();

            var groupsManager = new GroupInventoryManager(
                mockGroupsApiAccessor.Object,
                mockBenefitOptionsAccessor.Object,
                mockAgentApiAccessor.Object,
                mockLogger.Object,
                mockMapper.Object,
                mockFileExportEngine.Object);

            // Act
            var result = await groupsManager.GetGroupDetail(agentNumber, queryParams);

            var policy = mockGroupDetailResponse.Policies.First();
            var benefit = policy.Benefits.First();

            // Assert
            Assert.Single(benefit.CoverageOptions);
            Assert.Equal("Option2", benefit.CoverageOptions.First().Name);
        }
    }
}
