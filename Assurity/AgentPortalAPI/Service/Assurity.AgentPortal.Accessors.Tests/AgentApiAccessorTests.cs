namespace Assurity.AgentPortal.Accessors.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using Assurity.AgentPortal.Accessors.Agent;
using Assurity.AgentPortal.Contracts.AgentContracts;
using Assurity.AgentPortal.Service.Handlers;
using Assurity.Models.UnitedStates;
using global::Polly;
using global::Polly.Registry;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Xunit;
using AgentAPI = Assurity.Agent.Contracts;

[ExcludeFromCodeCoverage]
public class AgentApiAccessorTests
{
    private readonly Uri baseAddress = new Uri("http://localhost");

    [Fact]
    public async Task GetAgentAccess_Success()
    {
        // Arrange
        var agentId = "AAXB";
        var mockResponse = new AgentAPI.AgentAccessResponse
        {
            AccessLevel = AgentAPI.AccessLevel.Full
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.OK);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);
        var expectedUrl = baseAddress + $"agents/{agentId}/access";

        // Act
        var result = await agentApiAccessor.GetAgentAccess(agentId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockResponse.AccessLevel, result.AccessLevel);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAgentAccess_FailsWith404NoResponseBody_ThrowsException()
    {
        // Arrange
        var agentId = "AAXB";

        var mockHttpMessageHandler = GetMockHttpMessageHandler(null, HttpStatusCode.NotFound);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);
        var expectedUrl = baseAddress + $"agents/{agentId}/access";

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await agentApiAccessor.GetAgentAccess(agentId, CancellationToken.None));

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAgentAccess_FailsWith500_ThrowsException()
    {
        // Arrange
        var agentId = "AAXB";

        var mockHttpMessageHandler = GetMockHttpMessageHandler(null, HttpStatusCode.InternalServerError);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);
        var expectedUrl = baseAddress + $"agents/{agentId}/access";

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await agentApiAccessor.GetAgentAccess(agentId, CancellationToken.None));

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAdditionalAgentIds_Success()
    {
        // Arrange
        var mockResponse = new List<string>
        {
            "abc",
            "123",
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.OK);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);
        var expectedUrl = baseAddress + "agents/aaxb/additionalAgentIds";

        // Act
        var result = await agentApiAccessor.GetAdditionalAgentIds("aaxb");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mockResponse, result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAdditionalAgentIds_FailsWith404NoResponseBody_ThrowsException()
    {
        // Arrange
        var mockHttpMessageHandler = GetMockHttpMessageHandler(null, HttpStatusCode.NotFound);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);
        var expectedUrl = baseAddress + "agents/aaxb/additionalAgentIds";

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await agentApiAccessor.GetAdditionalAgentIds("aaxb"));

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAdditionalAgentIds_FailsWith404WithResponseBody_ReturnsEmptyObject()
    {
        // Arrange
        var mockHttpMessageHandler = GetMockHttpMessageHandler("we couldn't find your agent", HttpStatusCode.NotFound);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);
        var expectedUrl = baseAddress + "agents/aaxb/additionalAgentIds";

        // Act
        var result = await agentApiAccessor.GetAdditionalAgentIds("aaxb");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAgentContracts_Success()
    {
        // Arrange
        var agentContracts = new List<AgentAPI.AgentContract>
        {
            new AgentAPI.AgentContract { Level = "90", AgentNumber = "AC2021", MarketCode = "MXQ2035", CompanyCode = string.Empty },
            new AgentAPI.AgentContract { Level = "1", AgentNumber = "AC2021", MarketCode = "MXQ2035", CompanyCode = string.Empty },
            new AgentAPI.AgentContract { Level = "90", AgentNumber = "AC2021", MarketCode = "MXQ2045", CompanyCode = string.Empty },
            new AgentAPI.AgentContract { Level = "1", AgentNumber = "AC2021", MarketCode = "MXQ2045", CompanyCode = string.Empty }
        };

        var jsonResponse = JsonConvert.SerializeObject(agentContracts);

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await agentApiAccessor.GetAgentContracts("AC2021", false, AgentAPI.MarketCodeFilters.None, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        Assert.Equal("90", result[0].Level);
        Assert.Equal("AC2021", result[0].AgentNumber);
    }

    [Fact]
    public async Task GetAgentContracts_IfUnsuccessful_ShouldThrowException()
    {
        // Arrange
        var mockHttpMessageHandler = GetMockHttpMessageHandler(new StringContent(string.Empty), HttpStatusCode.InternalServerError);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            agentApiAccessor.GetAgentContracts("AC2021", false, AgentAPI.MarketCodeFilters.None, CancellationToken.None));
    }

    [Fact]
    public async Task GetAgentMarketCodes_Success()
    {
        // Arrange
        var agentId1 = "AC2021";

        var marketCode1 = "MXQ2035";
        var marketCode2 = "MXQ2045";

        var mockResponse = new List<AgentAPI.AgentContract>()
        {
            new()
            {
                AgentNumber = agentId1,
                MarketCode = marketCode1,
                Level = "90",
            },
            new()
            {
                AgentNumber = agentId1,
                MarketCode = marketCode2,
                Level = "90",
            }
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.OK);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);
        var expectedUrl = baseAddress + "agents/" + agentId1 + "/activeContracts?IncludeAssociatedAgentNumbers=True&MarketCodeFilter=none";

        // Act
        var result = await agentApiAccessor.GetAgentMarketCodes(agentId1, true, CancellationToken.None);

        // Assert
        var expectedMarketCodes = new List<string> { marketCode1, marketCode2 };
        Assert.Equal(expectedMarketCodes, result);

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAgentMarketCodes_IfNotFoundWithResponseBody_ShouldReturnEmptyList()
    {
        // Arrange
        var agentId = "12345678";
        var mockHttpMessageHandler = GetMockHttpMessageHandler("unable to find agent", HttpStatusCode.NotFound);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await agentApiAccessor.GetAgentMarketCodes(agentId, false, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAgentMarketCodes_IfNotFoundWithoutResponseBody_ShouldThrowException()
    {
        // Arrange
        var agentId = "12345678";
        var mockHttpMessageHandler = GetMockHttpMessageHandler(null, HttpStatusCode.NotFound);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await agentApiAccessor.GetAgentMarketCodes(agentId, false, CancellationToken.None));
    }

    [Fact]
    public async Task GetAgentInformation_Success()
    {
        // Arrange
        var agentId = "AC2021";

        var mockResponse = new AgentAPI.AgentInformation()
        {
            Name = new Assurity.Agent.Contracts.Name
            {
                IndividualFirst = "Jack",
                IndividualLast = "Smith",
                IndividualMiddle = "R",
                IndividualSuffix = "Jr",
                IsBusiness = false
            },
            Address = new Assurity.Agent.Contracts.Address
            {
                Line1 = "123 ABC St",
                City = "Lincoln",
                StateAbbreviation = State.NE.ToStateAbbreviation(),
                Zip = "68516",
                EmailAddress = "jsmithagent@assurity.com",
                PhoneNumber = "402-555-2222",
                FaxNumber = "402-000-0002"
            }
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.OK);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);
        var expectedUrl = baseAddress + "agents/" + agentId + "/information";

        // Act
        var result = await agentApiAccessor.GetAgentInformation(agentId, CancellationToken.None);

        // Assert
        Assert.False(result.Name.IsBusiness);
        Assert.Equal("Jack", result.Name.IndividualFirst);
        Assert.Equal("Smith", result.Name.IndividualLast);
        Assert.Equal("R", result.Name.IndividualMiddle);
        Assert.Equal("Jr", result.Name.IndividualSuffix);
        Assert.False(result.Name.IsBusiness);

        Assert.Equal("123 ABC St", result.Address.Line1);
        Assert.Equal("Lincoln", result.Address.City);
        Assert.Equal("NE", result.Address.StateAbbreviation);
        Assert.Equal("68516", result.Address.Zip);
        Assert.Equal("jsmithagent@assurity.com", result.Address.EmailAddress);
        Assert.Equal("402-555-2222", result.Address.PhoneNumber);
        Assert.Equal("402-000-0002", result.Address.FaxNumber);

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && expectedUrl.Equals(req.RequestUri.ToString())),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAgentInformation_WithParameters_Success()
    {
        // Arrange
        var agentNumber = "AC2021";
        var marketCode = "MXQ2035";
        var level = "1";
        var companyCode = "CompanyX";

        var mockResponse = new AgentAPI.AgentInformation
        {
            Name = new Assurity.Agent.Contracts.Name
            {
                IndividualFirst = "Jack",
                IndividualLast = "Smith",
                IndividualMiddle = "R",
                IndividualSuffix = "Jr",
                IsBusiness = false
            },
            Address = new Assurity.Agent.Contracts.Address
            {
                Line1 = "123 ABC St",
                City = "Lincoln",
                StateAbbreviation = State.NE.ToStateAbbreviation(),
                Zip = "68516",
                EmailAddress = "jsmithagent@assurity.com",
                PhoneNumber = "402-555-2222",
                FaxNumber = "402-000-0002"
            }
        };

        var jsonResponse = JsonConvert.SerializeObject(mockResponse);
        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.OK);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);
        var expectedUrl = $"agents/{agentNumber}/marketCode/{marketCode}/level/{level}/companyCode/{companyCode}/information";

        // Act
        var result = await agentApiAccessor.GetAgentInformation(agentNumber, marketCode, level, companyCode, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Jack", result.Name.IndividualFirst);
        Assert.Equal("Smith", result.Name.IndividualLast);
        Assert.Equal("123 ABC St", result.Address.Line1);
        Assert.Equal("Lincoln", result.Address.City);

        mockHttpMessageHandler.Protected().Verify(
            "SendAsync",
            Times.Exactly(1),
            ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri.ToString().EndsWith(expectedUrl)),
            ItExpr.IsAny<CancellationToken>());
    }

    [Fact]
    public async Task GetAgentInformation_IfApiCallReturnsNotFoundWithNoResponseBody_ShouldThrowException()
    {
        // Arrange
        var agentNumber = "AC2021";
        var marketCode = "MXQ2035";
        var level = "1";
        var companyCode = "CompanyX";
        var mockHttpMessageHandler = GetMockHttpMessageHandler(null, HttpStatusCode.NotFound);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await agentApiAccessor.GetAgentInformation(agentNumber, marketCode, level, companyCode, CancellationToken.None));
    }

    [Fact]
    public async Task GetAgentInformation_IfApiCallReturnsNotFoundWithResponseBody_ShouldReturnEmptyObject()
    {
        // Arrange
        var agentNumber = "AC2021";
        var marketCode = "MXQ2035";
        var level = "1";
        var companyCode = "CompanyX";
        var mockHttpMessageHandler = GetMockHttpMessageHandler("we can't find your agent", HttpStatusCode.NotFound);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await agentApiAccessor.GetAgentInformation(agentNumber, marketCode, level, companyCode, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<AgentAPI.ActiveHierarchy.AgentContractInformation>(result);
        Assert.Null(result.Address);
        Assert.Null(result.AdvanceFrequency);
        Assert.Null(result.AdvanceRate);
        Assert.Null(result.AntiMoneyLaundering);
        Assert.Null(result.ContractStatus);
    }

    [Fact]
    public async Task GetAgentHierarchy_Success()
    {
        // Arrange
        var agentNumber = "123456";
        var marketCode = "MKT";
        var level = "90";
        var companyCode = "CMP";
        ContractStatus? contractStatus = ContractStatus.Active;
        var includeAgentInformation = true;
        var includePendingRequirements = true;
        var filterAgentsWithoutPendingRequirements = true;

        var mockHierarchyResponse = new AgentAPI.ActiveHierarchy.ActiveHierarchyResponse
        {
            Hierarchy = new AgentAPI.ActiveHierarchy.AgentHierarchy(),
            Filters = new AgentAPI.ActiveHierarchy.ActiveHierarchyFilters()
        };

        var hierarchyJson = JsonConvert.SerializeObject(mockHierarchyResponse);
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("activeHierarchy")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(hierarchyJson)
            });

        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await agentApiAccessor.GetAgentHierarchy(
            agentNumber,
            marketCode,
            level,
            companyCode,
            contractStatus,
            includeAgentInformation,
            includePendingRequirements,
            filterAgentsWithoutPendingRequirements,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Hierarchy);
        Assert.NotNull(result.Filters);
    }

    [Fact]
    public async Task GetAgentHierarchyAsync_HierarchyApiError()
    {
        // Arrange
        var agentNumber = "123456";
        var marketCode = "MKT";
        var level = "90";
        var companyCode = "CMP";
        ContractStatus? contractStatus = ContractStatus.Active;
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("activeHierarchy")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("Not Found")
            });

        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await agentApiAccessor.GetAgentHierarchy(
            agentNumber,
            marketCode,
            level,
            companyCode,
            contractStatus,
            false,
            false,
            false,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.Filters);
        Assert.Null(result.Hierarchy);
    }

    [Fact]
    public async Task GetAgentHierarchyAsync_NoFilters()
    {
        // Arrange
        var agentNumber = "123456";
        var marketCode = "MKT";
        var level = "90";
        var companyCode = "CMP";
        ContractStatus? contractStatus = ContractStatus.Active;

        var mockHierarchyResponse = new AgentAPI.ActiveHierarchy.ActiveHierarchyResponse
        {
            Hierarchy = new AgentAPI.ActiveHierarchy.AgentHierarchy(),
            Filters = new AgentAPI.ActiveHierarchy.ActiveHierarchyFilters()
        };

        var hierarchyJson = JsonConvert.SerializeObject(mockHierarchyResponse);

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("activeHierarchy")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(hierarchyJson)
            });

        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await agentApiAccessor.GetAgentHierarchy(
            agentNumber,
            marketCode,
            level,
            companyCode,
            contractStatus,
            includeAgentInformation: false,
            includePendingRequirements: false,
            filterAgentsWithoutPendingRequirements: false,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Hierarchy);
        Assert.NotNull(result.Filters);
    }

    [Fact]
    public async Task GetActiveAppointmentsAsync_Success_ReturnsAppointments()
    {
        // Arrange
        var agentNumber = "123456";
        var marketCode = "MKT";
        var agentLevel = "90";
        var companyCode = "CMP";

        var currentDate = DateTime.UtcNow;

        var expectedResponse = new AgentAPI.ActiveHierarchy.AgentAppointmentResponse
        {
            Appointments = new List<Assurity.Agent.Contracts.AgentAppointment>
        {
            new Assurity.Agent.Contracts.AgentAppointment
            {
                AgentNumber = agentNumber,
                CompanyCode = companyCode,
                GrantedDate = currentDate,
                ExpirationDate = currentDate.AddYears(1),
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

        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
            });

        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await agentApiAccessor.GetActiveAppointments(agentNumber, marketCode, agentLevel, companyCode, null, 1, 20, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse.Appointments.Count, result.Appointments.Count);

        for (int i = 0; i < expectedResponse.Appointments.Count; i++)
        {
            var expectedAppointment = expectedResponse.Appointments[i];
            var actualAppointment = result.Appointments[i];

            Assert.Equal(expectedAppointment.AgentNumber, actualAppointment.AgentNumber);
            Assert.Equal(expectedAppointment.CompanyCode, actualAppointment.CompanyCode);
            Assert.True(Math.Abs((expectedAppointment.GrantedDate.Value - actualAppointment.GrantedDate.Value).TotalSeconds) < 1, "GrantedDate values are not equal");
            Assert.True(Math.Abs((expectedAppointment.ExpirationDate.Value - actualAppointment.ExpirationDate.Value).TotalSeconds) < 1, "ExpirationDate values are not equal");

            Assert.Equal(expectedAppointment.Name, actualAppointment.Name);
            Assert.Equal(expectedAppointment.IsResident, actualAppointment.IsResident);
            Assert.Equal(expectedAppointment.StateAbbreviation, actualAppointment.StateAbbreviation);
        }

        Assert.Equal(expectedResponse.AppointedStates, result.AppointedStates);
        Assert.Equal(expectedResponse.Page, result.Page);
        Assert.Equal(expectedResponse.PageSize, result.PageSize);
        Assert.Equal(expectedResponse.TotalPages, result.TotalPages);
    }

    [Fact]
    public async Task GetActiveAppointmentsAsync_ApiCallFails_ThrowsException()
    {
        // Arrange
        var agentNumber = "123456";
        var marketCode = "MKT";
        var agentLevel = "90";
        var companyCode = "CMP";
        var mockHttpMessageHandler = GetMockHttpMessageHandler(new StringContent("Internal Server Error"), HttpStatusCode.InternalServerError);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            agentApiAccessor.GetActiveAppointments(agentNumber, marketCode, agentLevel, companyCode, null, 1, 20, CancellationToken.None));
    }

    [Fact]
    public async Task GetDownlines_Success()
    {
        // Arrange
        var agentId = "AC2021";
        var marketCode = "US";
        var agentLevel = "1";
        var companyCode = "ABC";

        var mockResponse = new List<AgentAPI.AgentContract>
        {
            new AgentAPI.AgentContract
            {
                AgentNumber = "AC2021",
                MarketCode = "US",
                Level = "1",
                CompanyCode = "ABC"
            }
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.OK);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await agentApiAccessor.GetAgentHierarchyDownline(
            agentId,
            marketCode,
            agentLevel,
            companyCode,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        var agent = result.First();
        Assert.Equal("AC2021", agent.AgentNumber);
        Assert.Equal("US", agent.MarketCode);
        Assert.Equal("1", agent.Level);
        Assert.Equal("ABC", agent.CompanyCode);
    }

    [Fact]
    public async Task GetDownlines_EmptyAssociatedAgents()
    {
        // Arrange
        var agentId = "AC2021";
        var marketCode = "US";
        var agentLevel = "1";
        var companyCode = "ABC";
        var mockResponse = new List<AgentAPI.AgentContract>
        {
            new AgentAPI.AgentContract
            {
                AgentNumber = "AC2021",
                MarketCode = "US",
                Level = "1",
                CompanyCode = "ABC"
            }
        };

        var mockHttpMessageHandler = GetMockHttpMessageHandler(mockResponse, HttpStatusCode.OK);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await agentApiAccessor.GetAgentHierarchyDownline(agentId, marketCode, agentLevel, companyCode, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("AC2021", result[0].AgentNumber);
    }

    [Fact]
    public async Task GetDownlines_UnexpectedStatusCode_ThrowsException()
    {
        // Arrange
        var agentId = "AC2021";
        var marketCode = "US";
        var agentLevel = "1";
        var companyCode = "ABC";
        var mockHttpMessageHandler = GetMockHttpMessageHandler(null, HttpStatusCode.BadRequest);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            agentApiAccessor.GetAgentHierarchyDownline(agentId, marketCode, agentLevel, companyCode, CancellationToken.None));
    }

    [Fact]
    public async Task GetDownlines_NotFoundWithResponseBody_ReturnsEmptyList()
    {
        // Arrange
        var agentId = "AC2021";
        var marketCode = "US";
        var agentLevel = "1";
        var companyCode = "ABC";
        var mockHttpMessageHandler = GetMockHttpMessageHandler("Can't find agent", HttpStatusCode.NotFound);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await agentApiAccessor.GetAgentHierarchyDownline(agentId, marketCode, agentLevel, companyCode, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetDownlines_NotFoundWithoutResponseBody_ThrowsException()
    {
        // Arrange
        var agentId = "AC2021";
        var marketCode = "US";
        var agentLevel = "1";
        var companyCode = "ABC";
        var mockHttpMessageHandler = GetMockHttpMessageHandler(null, HttpStatusCode.NotFound);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(async () => await agentApiAccessor.GetAgentHierarchyDownline(agentId, marketCode, agentLevel, companyCode, CancellationToken.None));
    }

    [Fact]
    public async Task GetAgentHasHierarchyDownline_Success()
    {
        // Arrange
        var agentId = "AC2021";
        var mockHttpMessageHandler = GetMockHttpMessageHandler(true, HttpStatusCode.OK);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await agentApiAccessor.GetAgentHasHierarchyDownline(
            agentId,
            CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetAgentHasHierarchyDownline_UnexpectedStatusCode_ThrowsException()
    {
        // Arrange
        var agentId = "AC2021";
        var mockHttpMessageHandler = GetMockHttpMessageHandler(null, HttpStatusCode.BadRequest);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            agentApiAccessor.GetAgentHasHierarchyDownline(agentId, CancellationToken.None));
    }

    [Fact]
    public async Task GetAgentHasHierarchyDownline_NotFoundResponseHasBody_ReturnsFalse()
    {
        // Arrange
        var agentId = "AC2021";
        var mockHttpMessageHandler = GetMockHttpMessageHandler("something(Literally anything)", HttpStatusCode.NotFound);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act
        var result = await agentApiAccessor.GetAgentHasHierarchyDownline(agentId, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAgentHasHierarchyDownline_NotFoundNoBody_ThrowsException()
    {
        // Arrange
        var agentId = "AC2021";
        var mockHttpMessageHandler = GetMockHttpMessageHandler(null, HttpStatusCode.NotFound);
        var agentApiAccessor = GetAgentApiAccessor(mockHttpMessageHandler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() =>
            agentApiAccessor.GetAgentHasHierarchyDownline(agentId, CancellationToken.None));
    }

    private static Mock<HttpMessageHandler> GetMockHttpMessageHandler(
      object mockResponse,
      HttpStatusCode statusCode)
    {
        var httpResponseMessage = new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = mockResponse != null ? new StringContent(JsonConvert.SerializeObject(mockResponse)) : new StringContent(string.Empty)
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

    private AgentApiAccessor GetAgentApiAccessor(Mock<HttpMessageHandler> mockHttpMessageHandler)
    {
        var errorResponseHandler = new ErrorResponseHandler()
        {
            InnerHandler = mockHttpMessageHandler.Object
        };

        var mockhttpClient = new HttpClient(errorResponseHandler)
        {
            BaseAddress = baseAddress
        };

        var mockPipelineProvider = new Mock<ResiliencePipelineProvider<string>>();
        mockPipelineProvider
            .Setup(pipeline => pipeline.GetPipeline<HttpResponseMessage>(It.IsAny<string>()))
            .Returns(ResiliencePipeline<HttpResponseMessage>.Empty);
        return new AgentApiAccessor(mockhttpClient, mockPipelineProvider.Object, new Mock<ILogger<AgentApiAccessor>>().Object);
    }
}
