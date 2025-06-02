namespace Assurity.AgentPortal.Accessors.Tests;

using System.Text;
using Assurity.AgentPortal.Accessors.Impersonation;
using Assurity.AgentPortal.Accessors.MongoDb.Contracts;
using Assurity.AgentPortal.Accessors.Tests.MongoDb;
using Assurity.AgentPortal.Utilities.Configs;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using global::MongoDB.Bson;
using global::MongoDB.Driver;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class ImpersonationAccessorTests : MongoDbTestContext
{
    [Fact]
    public async Task ExecuteAgentSearch_ShouldReturnResults()
    {
        // Arrange
        var esHeaders = new Dictionary<string, IEnumerable<string>>()
        {
            { "x-elastic-product", new[] { "Elasticsearch" } }
        };
        var settings = new ElasticsearchClientSettings(
            new InMemoryRequestInvoker(Encoding.UTF8.GetBytes(ValidElasticsearchResponse()), headers: esHeaders));

        var client = new ElasticsearchClient(settings);

        var mockConfigManager = new Mock<IConfigurationManager>();
        mockConfigManager.Setup(x => x.ElasticSearchUserSearchIndex).Returns("test-user-serach-index");

        var mockLogger = new Mock<ILogger<ImpersonationAccessor>>();

        var accessor = new ImpersonationAccessor(mockLogger.Object, mockConfigManager.Object, client, null);

        // Act
        var response = await accessor.ExecuteAgentSearch("4hm", CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.NotEmpty(response);
    }

    [Fact]
    public async Task ExecuteAgentSearch_ShouldReturnEmpty()
    {
        // Arrange
        var esHeaders = new Dictionary<string, IEnumerable<string>>()
        {
            { "x-elastic-product", new[] { "Elasticsearch" } }
        };
        var settings = new ElasticsearchClientSettings(
            new InMemoryRequestInvoker(Encoding.UTF8.GetBytes(EmptyElasticsearchResponse()), headers: esHeaders));

        var client = new ElasticsearchClient(settings);

        var mockConfigManager = new Mock<IConfigurationManager>();
        mockConfigManager.Setup(x => x.ElasticSearchUserSearchIndex).Returns("test-user-serach-index");

        var mockLogger = new Mock<ILogger<ImpersonationAccessor>>();

        var accessor = new ImpersonationAccessor(mockLogger.Object, mockConfigManager.Object, client, null);

        // Act
        var response = await accessor.ExecuteAgentSearch("asvjo23", CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.Empty(response);
    }

    [Fact]
    public async Task GetUserSearchRecord_Success_ShouldReturnCorrectRecord()
    {
        // Arrange
        var records = await TempConnection.UserSearchCollection.FindAsync(FilterDefinition<UserSearch>.Empty);
        var testRecord = records.First();

        var configurationManager = new Mock<IConfigurationManager>();
        var impersonationAccessor = new ImpersonationAccessor(null, configurationManager.Object, null, TempConnection);

        // Act
        var result = await impersonationAccessor.GetUserSearchRecord(testRecord.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equivalent(testRecord, result);
    }

    [Fact]
    public async Task GetUserSearchRecord_NotFound_ShouldReturnNull()
    {
        // Arrange
        var configurationManager = new Mock<IConfigurationManager>();
        var impersonationAccessor = new ImpersonationAccessor(null, configurationManager.Object, null, TempConnection);

        // Act
        var result = await impersonationAccessor.GetUserSearchRecord(ObjectId.GenerateNewId().ToString());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task InsertImpersonationLog_Success()
    {
        // Arrange
        var userSearchRecords = TempConnection.UserSearchCollection.Find(FilterDefinition<UserSearch>.Empty);
        var testUserId = Guid.NewGuid().ToString();
        var configurationManager = new Mock<IConfigurationManager>();
        var impersonationAccessor = new ImpersonationAccessor(null, configurationManager.Object, null, TempConnection);

        // Act
        await impersonationAccessor.InsertImpersonationLog(testUserId, "test@test.com", userSearchRecords.First());

        // Assert
        var record = TempConnection.ImpersonationLogCollection.AsQueryable().Where(x => x.HomeOfficeUserId == testUserId).FirstOrDefault();
        Assert.NotNull(record);
        Assert.Equivalent(userSearchRecords.First(), record.ImpersonatedUser);
    }

    [Fact]
    public async Task GetRecentImpersonations_Success()
    {
        // Arrange
        var configurationManager = new Mock<IConfigurationManager>();

        var impersonationAccessor = new ImpersonationAccessor(null, configurationManager.Object, null, TempConnection);

        // Act
        var result = await impersonationAccessor.GetRecentImpersonations(HOMEOFFICETESTID, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetRecentImpersonations_NoRecords_ShouldReturnEmptyCollection()
    {
        // Arrange
        var configurationManager = new Mock<IConfigurationManager>();
        var impersonationAccessor = new ImpersonationAccessor(null, configurationManager.Object, null, TempConnection);

        // Act
        var result = await impersonationAccessor.GetRecentImpersonations(Guid.NewGuid().ToString(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    private static string ValidElasticsearchResponse()
    {
        return @"{
  ""took"": 1,
  ""timed_out"": false,
  ""_shards"": {
    ""total"": 2,
    ""successful"": 2,
    ""skipped"": 0,
    ""failed"": 0
  },
  ""hits"": {
    ""total"": {
      ""value"": 13,
      ""relation"": ""eq""
    },
    ""max_score"": 1,
    ""hits"": [
      {
        ""_index"": ""agentcenter.users"",
        ""_id"": ""6737bcf2fa26b41bfd7c6303"",
        ""_score"": 1,
        ""_source"": {
          ""PingUserId"": ""df6b1652-7de9-4a64-a782-0ef7949ab98c"",
          ""RegisteredAgentId"": ""4HM4"",
          ""Email"": ""test@assurity.com"",
          ""UserName"": ""user4HM4"",
          ""LastSynced"": {
            ""Ticks"": 638673028905033500,
            ""DateTime"": ""2024-11-15T21:28:10.503000"",
            ""Offset"": 0
          },
          ""AgentIds"": [
            ""4HM4""
          ],
          ""id"": ""6737bcf2fa26b41bfd7c6303"",
          ""_timestamp"": ""2024-11-25T21:54:52.977708+00:00"",
          ""Name"": ""VARNER INSURANCE GROUP, INC                       ""
        }
      },
      {
        ""_index"": ""agentcenter.users"",
        ""_id"": ""6737bcf2fa26b41bfd7c630e"",
        ""_score"": 1,
        ""_source"": {
          ""LastSynced"": {
            ""Ticks"": 638673028905033500,
            ""DateTime"": ""2024-11-15T21:28:10.503000"",
            ""Offset"": 0
          },
          ""AgentIds"": [
            ""4HMB""
          ],
          ""id"": ""6737bcf2fa26b41bfd7c630e"",
          ""_timestamp"": ""2024-11-25T21:54:52.977957+00:00""
        }
      },
      {
        ""_index"": ""agentcenter.users"",
        ""_id"": ""6737bcf2fa26b41bfd7c631d"",
        ""_score"": 1,
        ""_source"": {
          ""LastSynced"": {
            ""Ticks"": 638673028905033500,
            ""DateTime"": ""2024-11-15T21:28:10.503000"",
            ""Offset"": 0
          },
          ""AgentIds"": [
            ""4HMH""
          ],
          ""id"": ""6737bcf2fa26b41bfd7c631d"",
          ""_timestamp"": ""2024-11-25T21:54:52.978201+00:00""
        }
      },
      {
        ""_index"": ""agentcenter.users"",
        ""_id"": ""6737bcf2fa26b41bfd7c6326"",
        ""_score"": 1,
        ""_source"": {
          ""LastSynced"": {
            ""Ticks"": 638673028905033500,
            ""DateTime"": ""2024-11-15T21:28:10.503000"",
            ""Offset"": 0
          },
          ""AgentIds"": [
            ""4HML""
          ],
          ""id"": ""6737bcf2fa26b41bfd7c6326"",
          ""_timestamp"": ""2024-11-25T21:54:52.978319+00:00""
        }
      },
      {
        ""_index"": ""agentcenter.users"",
        ""_id"": ""6737bcf2fa26b41bfd7c6334"",
        ""_score"": 1,
        ""_source"": {
          ""LastSynced"": {
            ""Ticks"": 638673028905033500,
            ""DateTime"": ""2024-11-15T21:28:10.503000"",
            ""Offset"": 0
          },
          ""AgentIds"": [
            ""4HMR""
          ],
          ""id"": ""6737bcf2fa26b41bfd7c6334"",
          ""_timestamp"": ""2024-11-25T21:54:52.978554+00:00""
        }
      },
      {
        ""_index"": ""agentcenter.users"",
        ""_id"": ""6737bcf2fa26b41bfd7c6342"",
        ""_score"": 1,
        ""_source"": {
          ""LastSynced"": {
            ""Ticks"": 638673028905033500,
            ""DateTime"": ""2024-11-15T21:28:10.503000"",
            ""Offset"": 0
          },
          ""AgentIds"": [
            ""4HMV""
          ],
          ""id"": ""6737bcf2fa26b41bfd7c6342"",
          ""_timestamp"": ""2024-11-25T21:54:52.978794+00:00""
        }
      },
      {
        ""_index"": ""agentcenter.users"",
        ""_id"": ""6737bcf2fa26b41bfd7c6349"",
        ""_score"": 1,
        ""_source"": {
          ""LastSynced"": {
            ""Ticks"": 638673028905033500,
            ""DateTime"": ""2024-11-15T21:28:10.503000"",
            ""Offset"": 0
          },
          ""AgentIds"": [
            ""4HMW""
          ],
          ""id"": ""6737bcf2fa26b41bfd7c6349"",
          ""_timestamp"": ""2024-11-25T21:54:52.978911+00:00""
        }
      },
      {
        ""_index"": ""agentcenter.users"",
        ""_id"": ""6737bcf2fa26b41bfd7c62ff"",
        ""_score"": 1,
        ""_source"": {
          ""LastSynced"": {
            ""Ticks"": 638673028905033500,
            ""DateTime"": ""2024-11-15T21:28:10.503000"",
            ""Offset"": 0
          },
          ""AgentIds"": [
            ""4HM2""
          ],
          ""id"": ""6737bcf2fa26b41bfd7c62ff"",
          ""_timestamp"": ""2024-11-25T21:54:52.977589+00:00""
        }
      },
      {
        ""_index"": ""agentcenter.users"",
        ""_id"": ""6737bcf2fa26b41bfd7c6308"",
        ""_score"": 1,
        ""_source"": {
          ""LastSynced"": {
            ""Ticks"": 638673028905033500,
            ""DateTime"": ""2024-11-15T21:28:10.503000"",
            ""Offset"": 0
          },
          ""AgentIds"": [
            ""4HM7""
          ],
          ""id"": ""6737bcf2fa26b41bfd7c6308"",
          ""_timestamp"": ""2024-11-25T21:54:52.977839+00:00""
        }
      },
      {
        ""_index"": ""agentcenter.users"",
        ""_id"": ""6737bcf2fa26b41bfd7c6318"",
        ""_score"": 1,
        ""_source"": {
          ""LastSynced"": {
            ""Ticks"": 638673028905033500,
            ""DateTime"": ""2024-11-15T21:28:10.503000"",
            ""Offset"": 0
          },
          ""AgentIds"": [
            ""4HMF""
          ],
          ""id"": ""6737bcf2fa26b41bfd7c6318"",
          ""_timestamp"": ""2024-11-25T21:54:52.978083+00:00""
        }
      }
    ]
  }
}";
    }

    private static string EmptyElasticsearchResponse()
    {
        return @"{
  ""took"": 2,
  ""timed_out"": false,
  ""_shards"": {
    ""total"": 2,
    ""successful"": 2,
    ""skipped"": 0,
    ""failed"": 0
  },
  ""hits"": {
    ""total"": {
      ""value"": 0,
      ""relation"": ""eq""
    },
    ""max_score"": null,
    ""hits"": []
  }
}";
    }
}
