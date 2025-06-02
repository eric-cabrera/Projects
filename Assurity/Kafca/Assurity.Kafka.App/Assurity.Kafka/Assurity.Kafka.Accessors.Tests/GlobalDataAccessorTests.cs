namespace Assurity.Kafka.Accessors.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.Context;
    using Assurity.Kafka.Accessors.DataTransferObjects.Requirements;
    using Assurity.Kafka.Accessors.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class GlobalDataAccessorTests
    {
        private readonly DbContextOptions<GlobalDataContext> globalDataInMemoryOptions =
            new DbContextOptionsBuilder<GlobalDataContext>()
            .UseInMemoryDatabase(databaseName: "GlobalDataDatabase")
            .Options;

        private GlobalDataContext GlobalDataContext => new GlobalDataContext(globalDataInMemoryOptions);

        [TestCleanup]
        public void Dispose()
        {
            GlobalDataContext.Dispose();
            GlobalDataContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task GetJustInTimeAgentIds_Success()
        {
            // Arrange
            var mockDbContextFactory = GetMockGlobalDataContextFactory();

            var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

            var nbFolderIds = new List<string>
            {
                "abc123",
            };

            // Act
            var results = await globalDataAccessor.GetJustInTimeAgentIds(nbFolderIds);

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
        }

        [TestMethod]
        public async Task GetJustInTimeAgentIds_Success_MultipleAgentIds()
        {
            // Arrange
            var mockDbContextFactory = GetMockGlobalDataContextFactory();

            var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

            var nbFolderIds = new List<string>
            {
                "abc456",
            };

            // Act
            var result = await globalDataAccessor.GetJustInTimeAgentIds(nbFolderIds);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public async Task GetAgentFolderId_Success()
        {
            // Arrange
            var mockDbContextFactory = GetMockGlobalDataContextFactory();

            var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

            // Act
            var results = await globalDataAccessor.GetAgentFolderIds("123456");

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("1", results[0]);
        }

        [TestMethod]
        public async Task GetAgentFolderIdFromAttributes_Success()
        {
            // Arrange
            var mockDbContextFactory = GetMockGlobalDataContextFactory();

            var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

            // Act
            var results = await globalDataAccessor.GetAgentFolderIdsFromAttributes("123456");

            // Assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("24110TST100012F", results[0]);
        }

        [TestMethod]
        public async Task GetQueueFromFolderId_Success()
        {
            // Arrange
            var mockDbContextFactory = GetMockGlobalDataContextFactory();

            var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

            // Act
            var result = await globalDataAccessor.GetQueueFromFolderId("1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Queue 1", result);
        }

        [TestMethod]
        public async Task GetJitAgentInfoFromFolderId_Success()
        {
            // Arrange
            var mockDbContextFactory = GetMockGlobalDataContextFactory();

            var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

            // Act
            var result = await globalDataAccessor.GetJitAgentInfoFromFolderId("123456", "1", "IS", "01");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("01", result.Level);
            Assert.AreEqual("50", result.UplineLevel);
        }

        [TestMethod]
        public async Task GetJitAgentInfoFromFolderId_NotFoundReturnsEmptyObject()
        {
            // Arrange
            var mockDbContextFactory = GetMockGlobalDataContextFactory();

            var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

            // Act
            var result = await globalDataAccessor.GetJitAgentInfoFromFolderId("abc123", "800", "IS", "01");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.AgentId);
        }

        [TestMethod]
        public async Task GetRequirementComment_Success()
        {
            // Arrange
            var mockDbContextFactory = GetMockGlobalDataContextFactory();
            var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

            // Act
            var result = await globalDataAccessor.GetRequirementComment("123456789", 1, 5, "CASE APP");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Please note that the applicant may also qualify for a Business Overhead Expense policy.", result);
        }

        [TestMethod]
        public void GetRequirementComments_Success()
        {
            // Arrange
            var mockDbContextFactory = GetMockGlobalDataContextFactory();
            var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);
            var policyNumber = "123456789";
            var reqSeq = 1;
            var ix = 5;
            var reqType = "CASE APP";
            var lookupDto = new GlobalRequirementCommentsLookupDTO
            {
                PolicyNumber = policyNumber,
                Lookups = new List<RequirementLookupDTO>
                {
                    new RequirementLookupDTO
                    {
                        REQSEQ = reqSeq,
                        IX = ix,
                        REQTYPE = reqType
                    }
                }
            };

            // Act
            var result = globalDataAccessor.GetRequirementComments(lookupDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Single().Note, "Please note that the applicant may also qualify for a Business Overhead Expense policy.");
        }

        [TestMethod]
        public async Task GetRequirementComment_NotFoundReturnsNull()
        {
            // Arrange
            var mockDbContextFactory = GetMockGlobalDataContextFactory();
            var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

            // Act
            var result = await globalDataAccessor.GetRequirementComment("123456789", 10, 5, "CASE APP");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetPolicyNumber_RecordFound_ShouldReturnPolicyNumber()
        {
            // Arrange
            var mockDbContextFactory = GetMockGlobalDataContextFactory();
            var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

            // Act
            var policyNumber = await globalDataAccessor
                .GetPolicyNumber("abc123");

            // Assert
            Assert.AreEqual("1234567890", policyNumber);
        }

        [TestMethod]
        public async Task GetPolicyNumber_NoRecordFound_ShouldReturnNull()
        {
            // Arrange
            var mockDbContextFactory = GetMockGlobalDataContextFactory();
            var globalDataAccessor = new GlobalDataAccessor(mockDbContextFactory.Object);

            // Act
            var policyNumber = await globalDataAccessor
                .GetPolicyNumber("Bogus");

            // Assert
            Assert.IsNull(policyNumber);
        }

        private void SeedData(GlobalDataContext context)
        {
            context.SysZ9processes.AddRange(
                new List<SysZ9Process>
                {
                    new SysZ9Process
                    {
                        RECORDID = 1,
                        NBFOLDEROBJID = "abc123",
                        AGENTID = "123456",
                        AGENTMARKETCODE = "IS",
                        AGENTLEVEL = "01"
                    },
                    new SysZ9Process
                    {
                        RECORDID = 2,
                        NBFOLDEROBJID = "abc456",
                        AGENTID = "654321",
                        AGENTMARKETCODE = "IS",
                        AGENTLEVEL = "01"
                    },
                    new SysZ9Process
                    {
                        RECORDID = 3,
                        NBFOLDEROBJID = "abc456",
                        AGENTID = "abc123",
                        AGENTMARKETCODE = "IS",
                        AGENTLEVEL = "01"
                    },
                });

            context.SysACAgentData.AddRange(
                new List<SysACAgentData>
                {
                    new SysACAgentData
                    {
                        FOLDERID = "1",
                        AGENTID = "123456",
                        FIRSTNAME = "First",
                        MIDDLENAME = "Middle",
                        LASTNAME = "Last",
                        BUSINESSNAME = "Business"
                    },
                    new SysACAgentData
                    {
                        FOLDERID = "2",
                        AGENTID = "654321",
                        FIRSTNAME = "First",
                        MIDDLENAME = "Middle",
                        LASTNAME = "Last",
                        BUSINESSNAME = "Business"
                    },
                });

            context.Attributes.AddRange(
                new List<Attributes>
                {
                    new Attributes
                    {
                        AgentID = "123456",
                        OBJECT_TYPE = 3,
                        OBJECT_ID = "24110TST100012F"
                    },
                    new Attributes
                    {
                        AgentID = "654321",
                        OBJECT_TYPE = 3,
                        OBJECT_ID = "24110TST5551567"
                    },
                });

            context.Queues.AddRange(
                new List<QUEUES>
                {
                    new QUEUES
                    {
                        ID = "1",
                        QUEUE = "Queue 1",
                    },
                    new QUEUES
                    {
                        ID = "2",
                        QUEUE = "Queue 2",
                    },
                });

            context.SysACAgentMarketCodes.AddRange(
                new List<SysACAgentMarketCodes>
                {
                    new SysACAgentMarketCodes
                    {
                        FOLDERID = "1",
                        MARKETCODE = "IS",
                        CONTRACTLEVEL = "01",
                        PENDINGRPTDISABLED = 0,
                        UPLINEAGENTID = "abc123",
                        UPLINECONTRACTLEVEL = "50",
                        UPLINEMARKETCODE = "IS"
                    },
                    new SysACAgentMarketCodes
                    {
                        FOLDERID = "2",
                        MARKETCODE = "IS",
                        CONTRACTLEVEL = "01",
                    }
                });

            context.SysNBRequirements.AddRange(
                new List<SysNBRequirements>
                {
                    new SysNBRequirements
                    {
                        POLICYNUMBER = "123456789",
                        REQSEQ = 1,
                        IX = 5,
                        REQTYPE = "CASE APP",
                        REQNOTE = "Please note that the applicant may also qualify for a Business Overhead Expense policy."
                    },
                    new SysNBRequirements
                    {
                        POLICYNUMBER = "123456789",
                        REQSEQ = 2,
                        IX = 6,
                        REQTYPE = "APS",
                        REQNOTE = "Dr Henning"
                    },
                    new SysNBRequirements
                    {
                        POLICYNUMBER = "345678912",
                        REQSEQ = 1,
                        IX = 2,
                        REQTYPE = "APS",
                        REQNOTE = "Dr.Channing"
                    },
                    new SysNBRequirements
                    {
                        POLICYNUMBER = "345678912",
                        REQSEQ = 2,
                        IX = 1,
                        REQTYPE = "APS(2)",
                        REQNOTE = "Urology Associates Of Central Nebraska"
                    },
                    new SysNBRequirements
                    {
                        POLICYNUMBER = "567891234",
                        REQSEQ = 1,
                        IX = 1,
                        REQTYPE = "DEL REQT",
                        REQNOTE = "Signature"
                    },
                    new SysNBRequirements
                    {
                        POLICYNUMBER = "567891234",
                        REQSEQ = 2,
                        IX = 1,
                        REQTYPE = "PAC",
                        REQNOTE = "Received"
                    }
                });

            context.VAttributesunionArcs.AddRange(
                new List<VAttributesunionArc>
                {
                    new VAttributesunionArc
                    {
                        ObjectId = "abc123",
                        ObjectType = 0,
                        PolicyNumber = "1234567890"
                    }
                });

            context.SaveChanges();
        }

        private IMock<IDbContextFactory<GlobalDataContext>> GetMockGlobalDataContextFactory()
        {
            var mockDbContextFactory = new Mock<IDbContextFactory<GlobalDataContext>>(MockBehavior.Strict);
            mockDbContextFactory
                .Setup(dbContextFactory => dbContextFactory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(GlobalDataContext);

            mockDbContextFactory
                .Setup(dbContextFactory => dbContextFactory.CreateDbContext())
                .Returns(GlobalDataContext);

            SeedData(GlobalDataContext);

            return mockDbContextFactory;
        }
    }
}
