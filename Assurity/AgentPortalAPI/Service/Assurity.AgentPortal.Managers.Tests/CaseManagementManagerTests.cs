namespace Assurity.AgentPortal.Managers.Tests;

using System.Diagnostics.CodeAnalysis;
using Assurity.AgentPortal.Accessors.Agent;
using Assurity.AgentPortal.Accessors.ApplicationTracker;
using Assurity.AgentPortal.Contracts.CaseManagement;
using Assurity.AgentPortal.Managers.CaseManagement;
using Assurity.AgentPortal.Managers.Claims.Mapping;
using Assurity.ApplicationTracker.Contracts;
using Assurity.ApplicationTracker.Contracts.Enums;
using AutoMapper;
using Moq;
using Xunit;

using AppTrackerDTO = Assurity.ApplicationTracker.Contracts.DataTransferObjects;

[ExcludeFromCodeCoverage]
public class CaseManagementManagerTests
{
    private readonly Mapper mapper;

    public CaseManagementManagerTests()
    {
        var mapperProfiles = new List<Profile>
        {
            new CaseManagementMappingProfile()
        };

        var mapperConfig = new MapperConfiguration(config => config.AddProfiles(mapperProfiles));
        mapper = new Mapper(mapperConfig);
    }

    [Fact]
    public async Task GetCases_Success()
    {
        var agentId = "agent123";
        var caseManagementParameters = new CaseManagementParameters
        {
            PageNumber = 1,
            PageSize = 1
        };
        var cancellationToken = CancellationToken.None;

        var mockApplicationTrackerApiAccessor = new Mock<IApplicationTrackerApiAccessor>();
        mockApplicationTrackerApiAccessor.Setup(accessor => accessor.GetCases(agentId, caseManagementParameters, cancellationToken))
            .ReturnsAsync(() => GetMockCaseManagementResponse());

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(agentId, cancellationToken))
            .ReturnsAsync(() => GetMockAgentApiResponse());

        var mockDocuSignApiAccessor = new Mock<IDocuSignApiAccessor>();

        var caseManagementManager = new CaseManagementManager(mapper, mockApplicationTrackerApiAccessor.Object, mockAgentApiAccessor.Object, mockDocuSignApiAccessor.Object);

        var response = await caseManagementManager.GetCases(agentId, caseManagementParameters);

        var expectedResponse = GetMockCaseManagementResponse();

        Assert.NotNull(response);
        Assert.Equal(expectedResponse.Data.Count, response.Cases.Count);

        mockApplicationTrackerApiAccessor.Verify(
            accessor => accessor.GetCases(
                agentId,
                caseManagementParameters,
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetCases_NullResponse_ShouldReturn_Null()
    {
        var agentId = "agent123";
        var caseManagementParameters = new CaseManagementParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var cancellationToken = CancellationToken.None;

        var mockApplicationTrackerApiAccessor = new Mock<IApplicationTrackerApiAccessor>();
        mockApplicationTrackerApiAccessor.Setup(accessor => accessor.GetCases(agentId, caseManagementParameters, cancellationToken))
            .ReturnsAsync(() => null);

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(agentId, cancellationToken))
            .ReturnsAsync(() => GetMockAgentApiResponse());

        var mockDocuSignApiAccessor = new Mock<IDocuSignApiAccessor>();

        var caseManagementManager = new CaseManagementManager(mapper, mockApplicationTrackerApiAccessor.Object, mockAgentApiAccessor.Object, mockDocuSignApiAccessor.Object);

        var response = await caseManagementManager.GetCases(agentId, caseManagementParameters);

        Assert.Null(response);

        mockApplicationTrackerApiAccessor.Verify(
            accessor => accessor.GetCases(
                agentId,
                caseManagementParameters,
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetCases_Should_Filter_RecipientSent_And_RecipientCompleted_Based_On_RecipientId()
    {
        var agentId = "agent123";
        var caseManagementParameters = new CaseManagementParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var cancellationToken = CancellationToken.None;

        var mockApplicationTrackerApiAccessor = new Mock<IApplicationTrackerApiAccessor>();
        mockApplicationTrackerApiAccessor.Setup(accessor => accessor.GetCases(agentId, caseManagementParameters, cancellationToken))
            .ReturnsAsync(() => new PagedEvents
            {
                Data = new List<AppTrackerDTO.ApplicationTracker>
                {
                new AppTrackerDTO.ApplicationTracker
                {
                    CaseId = "12345",
                    Product = "ad",
                    SignedInformation = new AppTrackerDTO.SignedInformation
                    {
                        Signers = new List<AppTrackerDTO.Signer>
                        {
                            new AppTrackerDTO.Signer { RecipientId = 1 }
                        }
                    },
                    Events = new List<AppTrackerDTO.AppTrackerEvent>
                    {
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = DateTime.UtcNow.AddHours(-2),
                            Event = EventType.RecipientSent,
                            RecipientId = 1
                        },
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = DateTime.UtcNow.AddHours(-1),
                            Event = EventType.RecipientCompleted,
                            RecipientId = 2
                        },
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = DateTime.UtcNow,
                            Event = EventType.InterviewStarted
                        }
                    }
                }
                },
                NumberOfPages = 1
            });

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(agentId, cancellationToken))
            .ReturnsAsync(() => GetMockAgentApiResponse());

        var mockDocuSignApiAccessor = new Mock<IDocuSignApiAccessor>();

        var caseManagementManager = new CaseManagementManager(mapper, mockApplicationTrackerApiAccessor.Object, mockAgentApiAccessor.Object, mockDocuSignApiAccessor.Object);

        var response = await caseManagementManager.GetCases(agentId, caseManagementParameters);

        Assert.NotNull(response);
        Assert.Single(response.Cases);

        var caseItem = response.Cases.First();

        // Verify that only the matching "RecipientSent" and other general events are included
        Assert.Equal(2, caseItem.Events.Count);
        Assert.Contains(caseItem.Events, e => e.Event == EventType.RecipientSent && e.RecipientId == 1);
        Assert.DoesNotContain(caseItem.Events, e => e.Event == EventType.RecipientCompleted && e.RecipientId == 2);
        Assert.Contains(caseItem.Events, e => e.Event == EventType.InterviewStarted);
    }

    [Fact]
    public async Task GetCases_Should_Exclude_Events_With_Empty_EventName()
    {
        var agentId = "agent123";
        var caseManagementParameters = new CaseManagementParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var cancellationToken = CancellationToken.None;

        var mockApplicationTrackerApiAccessor = new Mock<IApplicationTrackerApiAccessor>();
        mockApplicationTrackerApiAccessor.Setup(accessor => accessor.GetCases(agentId, caseManagementParameters, cancellationToken))
            .ReturnsAsync(() => new PagedEvents
            {
                Data = new List<AppTrackerDTO.ApplicationTracker>
                {
                new AppTrackerDTO.ApplicationTracker
                {
                    CaseId = "12345",
                    Product = "ad",
                    Events = new List<AppTrackerDTO.AppTrackerEvent>
                    {
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = DateTime.UtcNow,
                        },
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = DateTime.UtcNow,
                            Event = EventType.InterviewStarted
                        }
                    }
                }
                },
                NumberOfPages = 1
            });

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(agentId, cancellationToken))
            .ReturnsAsync(() => GetMockAgentApiResponse());

        var mockDocuSignApiAccessor = new Mock<IDocuSignApiAccessor>();

        var caseManagementManager = new CaseManagementManager(mapper, mockApplicationTrackerApiAccessor.Object, mockAgentApiAccessor.Object, mockDocuSignApiAccessor.Object);

        var response = await caseManagementManager.GetCases(agentId, caseManagementParameters);

        Assert.NotNull(response);
        Assert.Single(response.Cases);

        var caseItem = response.Cases.First();
        Assert.Single(caseItem.Events);
    }

    [Fact]
    public async Task GetCases_Should_Include_RecipientSent_And_RecipientCompleted_Events_With_Null_RecipientId()
    {
        var agentId = "agent123";
        var caseManagementParameters = new CaseManagementParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var cancellationToken = CancellationToken.None;

        var mockApplicationTrackerApiAccessor = new Mock<IApplicationTrackerApiAccessor>();
        mockApplicationTrackerApiAccessor.Setup(accessor => accessor.GetCases(agentId, caseManagementParameters, cancellationToken))
            .ReturnsAsync(() => new PagedEvents
            {
                Data = new List<AppTrackerDTO.ApplicationTracker>
                {
                new AppTrackerDTO.ApplicationTracker
                {
                    CaseId = "12345",
                    Product = "ad",
                    SignedInformation = new AppTrackerDTO.SignedInformation
                    {
                        Signers = new List<AppTrackerDTO.Signer>
                        {
                            new AppTrackerDTO.Signer { RecipientId = 1 }
                        }
                    },
                    Events = new List<AppTrackerDTO.AppTrackerEvent>
                    {
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = DateTime.UtcNow,
                            Event = EventType.RecipientSent,
                            RecipientId = null
                        },
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = DateTime.UtcNow.AddMinutes(-5),
                            Event = EventType.InterviewStarted
                        }
                    }
                }
                },
                NumberOfPages = 1
            });

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(agentId, cancellationToken))
            .ReturnsAsync(() => GetMockAgentApiResponse());

        var mockDocuSignApiAccessor = new Mock<IDocuSignApiAccessor>();

        var caseManagementManager = new CaseManagementManager(mapper, mockApplicationTrackerApiAccessor.Object, mockAgentApiAccessor.Object, mockDocuSignApiAccessor.Object);

        var response = await caseManagementManager.GetCases(agentId, caseManagementParameters);

        Assert.NotNull(response);
        Assert.Single(response.Cases);

        var caseItem = response.Cases.First();
        Assert.Equal(2, caseItem.Events.Count());
    }

    [Fact]
    public async Task GetCases_Should_Map_ProductNames_And_EventNames()
    {
        var agentId = "agent123";
        var caseManagementParameters = new CaseManagementParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var cancellationToken = CancellationToken.None;

        var mockApplicationTrackerApiAccessor = new Mock<IApplicationTrackerApiAccessor>();
        mockApplicationTrackerApiAccessor.Setup(accessor => accessor.GetCases(agentId, caseManagementParameters, cancellationToken))
            .ReturnsAsync(() => new PagedEvents
            {
                Data = new List<AppTrackerDTO.ApplicationTracker>
                {
                new AppTrackerDTO.ApplicationTracker
                {
                    CaseId = "12345",
                    Product = "ad",
                    Events = new List<AppTrackerDTO.AppTrackerEvent>
                    {
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = DateTime.UtcNow,
                            Event = EventType.InterviewStarted
                        },
                    }
                }
                },
                NumberOfPages = 1
            });

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(agentId, cancellationToken))
            .ReturnsAsync(() => GetMockAgentApiResponse());

        var mockDocuSignApiAccessor = new Mock<IDocuSignApiAccessor>();

        var caseManagementManager = new CaseManagementManager(mapper, mockApplicationTrackerApiAccessor.Object, mockAgentApiAccessor.Object, mockDocuSignApiAccessor.Object);

        var response = await caseManagementManager.GetCases(agentId, caseManagementParameters);

        Assert.NotNull(response);
        Assert.Single(response.Cases);

        var caseItem = response.Cases.First();
        Assert.Equal("Accidental Death", caseItem.ProductName);

        var eventItem = caseItem.Events.First();
        Assert.Equal("Interview Started", eventItem.EventName);
    }

    [Fact]
    public async Task GetCases_EmptyData_ShouldReturn_EmptyResponse()
    {
        var agentId = "agent123";
        var caseManagementParameters = new CaseManagementParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var cancellationToken = CancellationToken.None;

        var mockApplicationTrackerApiAccessor = new Mock<IApplicationTrackerApiAccessor>();
        mockApplicationTrackerApiAccessor.Setup(accessor => accessor.GetCases(agentId, caseManagementParameters, cancellationToken))
            .ReturnsAsync(() => new PagedEvents
            {
                Data = new List<AppTrackerDTO.ApplicationTracker>(),
                NumberOfPages = 1
            });

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(agentId, cancellationToken))
            .ReturnsAsync(() => GetMockAgentApiResponse());

        var mockDocuSignApiAccessor = new Mock<IDocuSignApiAccessor>();

        var caseManagementManager = new CaseManagementManager(mapper, mockApplicationTrackerApiAccessor.Object, mockAgentApiAccessor.Object, mockDocuSignApiAccessor.Object);

        var response = await caseManagementManager.GetCases(agentId, caseManagementParameters);

        Assert.NotNull(response);
        Assert.Empty(response.Cases);
    }

    [Fact]
    public async Task GetCases_WhenApplicationTrackerThrowsException_ShouldThrow()
    {
        var agentId = "agent123";
        var caseManagementParameters = new CaseManagementParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var cancellationToken = CancellationToken.None;

        var mockApplicationTrackerApiAccessor = new Mock<IApplicationTrackerApiAccessor>();
        mockApplicationTrackerApiAccessor.Setup(accessor => accessor.GetCases(agentId, caseManagementParameters, cancellationToken))
            .ThrowsAsync(new Exception("Error fetching cases"));

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(agentId, cancellationToken))
            .ReturnsAsync(() => GetMockAgentApiResponse());

        var mockDocuSignApiAccessor = new Mock<IDocuSignApiAccessor>();

        var caseManagementManager = new CaseManagementManager(mapper, mockApplicationTrackerApiAccessor.Object, mockAgentApiAccessor.Object, mockDocuSignApiAccessor.Object);

        await Assert.ThrowsAsync<Exception>(async () => await caseManagementManager.GetCases(agentId, caseManagementParameters));

        mockApplicationTrackerApiAccessor.Verify(
            accessor => accessor.GetCases(
                agentId,
                caseManagementParameters,
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetFilterOptions_Should_Return_FilterOptions_And_AgentIds()
    {
        var agentId = "agent123";
        var cancellationToken = CancellationToken.None;

        var mockApplicationTrackerApiAccessor = new Mock<IApplicationTrackerApiAccessor>();
        mockApplicationTrackerApiAccessor.Setup(accessor => accessor.GetFilterOptions(agentId, cancellationToken))
            .ReturnsAsync(new EventFilterOptions
            {
                PrimaryInsuredName = new List<string> { "John Doe", "Jane Doe" }
            });

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(agentId, cancellationToken))
            .ReturnsAsync(new List<string> { "agent1", "agent2" });

        var mockDocuSignApiAccessor = new Mock<IDocuSignApiAccessor>();

        var caseManagementManager = new CaseManagementManager(mapper, mockApplicationTrackerApiAccessor.Object, mockAgentApiAccessor.Object, mockDocuSignApiAccessor.Object);

        var response = await caseManagementManager.GetFilterOptions(agentId, cancellationToken);

        Assert.NotNull(response);
        Assert.Equal(2, response.ViewAsAgentIds.Count);
        Assert.Equal(2, response.PrimaryInsuredNames.Count);
        Assert.Contains("John Doe", response.PrimaryInsuredNames);
    }

    [Fact]
    public async Task GetCases_Should_Filter_Duplicate_Events_And_Keep_Latest()
    {
        var agentId = "agent123";
        var caseManagementParameters = new CaseManagementParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var cancellationToken = CancellationToken.None;

        var mockApplicationTrackerApiAccessor = new Mock<IApplicationTrackerApiAccessor>();
        mockApplicationTrackerApiAccessor.Setup(accessor => accessor.GetCases(agentId, caseManagementParameters, cancellationToken))
            .ReturnsAsync(() => new PagedEvents
            {
                Data = new List<AppTrackerDTO.ApplicationTracker>
                {
                new AppTrackerDTO.ApplicationTracker
                {
                    CaseId = "12345",
                    Product = "ad",
                    Events = new List<AppTrackerDTO.AppTrackerEvent>
                    {
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = new DateTime(2023, 02, 09, 18, 56, 47, DateTimeKind.Utc),
                            Event = EventType.ReceivedQuote
                        },
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = new DateTime(2023, 02, 09, 19, 03, 25, DateTimeKind.Utc),
                            Event = EventType.ReceivedQuote
                        },
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = new DateTime(2023, 02, 09, 19, 02, 01, DateTimeKind.Utc),
                            Event = EventType.ReceivedQuote
                        }
                    }
                }
                },
                NumberOfPages = 1
            });

        var mockAgentApiAccessor = new Mock<IAgentApiAccessor>();
        mockAgentApiAccessor.Setup(accessor => accessor.GetAdditionalAgentIds(agentId, cancellationToken))
            .ReturnsAsync(() => GetMockAgentApiResponse());

        var mockDocuSignApiAccessor = new Mock<IDocuSignApiAccessor>();

        var caseManagementManager = new CaseManagementManager(mapper, mockApplicationTrackerApiAccessor.Object, mockAgentApiAccessor.Object, mockDocuSignApiAccessor.Object);

        var response = await caseManagementManager.GetCases(agentId, caseManagementParameters);

        Assert.NotNull(response);
        Assert.Single(response.Cases);

        var caseItem = response.Cases.First();

        Assert.Single(caseItem.Events);
        Assert.Equal(new DateTime(2023, 02, 09, 19, 03, 25, DateTimeKind.Utc), caseItem.Events.First().CreatedDateTime);
    }

    private PagedEvents GetMockCaseManagementResponse()
    {
        var mockCaseManagementResponse = new PagedEvents
        {
            Data = new List<AppTrackerDTO.ApplicationTracker>
            {
                new AppTrackerDTO.ApplicationTracker
                {
                    CaseId = "12345",
                    Events = new List<AppTrackerDTO.AppTrackerEvent>
                    {
                        new AppTrackerDTO.AppTrackerEvent
                        {
                            CreatedDateTime = DateTime.UtcNow,
                            Event = EventType.InterviewStarted
                        },
                    }
                }
            },
            NumberOfPages = 1
        };

        return mockCaseManagementResponse;
    }

    private List<string> GetMockAgentApiResponse()
    {
        var mockResponse = new List<string>
        {
            "123",
            "456"
        };

        return mockResponse;
    }
}
