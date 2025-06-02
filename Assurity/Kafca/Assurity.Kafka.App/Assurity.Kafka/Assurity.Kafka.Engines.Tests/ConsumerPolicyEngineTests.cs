namespace Assurity.Kafka.Engines.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Mapping;
    using Assurity.Kafka.Engines.Policy;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ConsumerPolicyEngineTests
    {
        private Mock<ILogger<ConsumerPolicyEngine>> mockLogger = new Mock<ILogger<ConsumerPolicyEngine>>();

        [TestMethod]
        public async Task GetAgents_EmptyAgentRecords_ShouldReturnEmptyList()
        {
            // Arrange
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetPolicyAgentDTOs(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new List<PolicyAgentDTO>());

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                mockLogger.Object,
                mockPolicyMapper.Object,
                null);

            var applicationDate = new DateTime(2022, 01, 01);

            // Act
            var agents = await policyEngine.GetAgents("1122334455", "01", applicationDate);

            // Assert
            Assert.IsNotNull(agents);
            Assert.AreEqual(0, agents.Count);
        }

        [TestMethod]
        public async Task GetAgents_DataExists_ShouldReturnAgents_Without_JITAgents()
        {
            // Arrange
            var policyNumber = "1234567890";
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            var nonJitAgent = new PolicyAgentDTO
            {
                AgentNumber = "ABC1"
            };

            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetPolicyAgentDTOs(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new List<PolicyAgentDTO>
                {
                    nonJitAgent,
                });

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapAgent(nonJitAgent))
                .Returns(new Agent { AgentId = nonJitAgent.AgentNumber });

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                null,
                mockPolicyMapper.Object,
                null);

            var applicationDate = new DateTime(2022, 01, 01);

            // Act
            var agents = await policyEngine.GetAgents(policyNumber, "01", applicationDate);

            // Assert
            Assert.IsNotNull(agents);
            Assert.AreEqual(1, agents.Count);
            Assert.IsNotNull(agents.SingleOrDefault(a => a.AgentId == nonJitAgent.AgentNumber));
        }

        [TestMethod]
        public async Task GetAgents_DataExists_ShouldReturn_Agent_Including_JITAgents()
        {
            // Arrange
            var policyNumber = "1234567890";
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            var nonJitAgent = new PolicyAgentDTO
            {
                AgentNumber = "ABC1"
            };

            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetPolicyAgentDTOs(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(new List<PolicyAgentDTO>
                {
                    nonJitAgent,
                    new PolicyAgentDTO
                    {
                        AgentNumber = AgentType.Z9Agent
                    }
                });

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapAgent(nonJitAgent))
                .Returns(new Agent { AgentId = nonJitAgent.AgentNumber });

            var expectedQueueDescription = "AC AdvChgDocRecv";
            var mockSupportDataAccessor = new Mock<ISupportDataAccessor>(MockBehavior.Strict);
            mockSupportDataAccessor
                .Setup(supportDataAccessor => supportDataAccessor.GetQueueDescriptions())
                .ReturnsAsync(new List<string?> { expectedQueueDescription });

            var mockGlobalDataAccessor = new Mock<IGlobalDataAccessor>(MockBehavior.Strict);
            mockGlobalDataAccessor
                .Setup(globalDataAccessor => globalDataAccessor.GetNewBusinessFolderIds(policyNumber))
                .ReturnsAsync(new List<string> { "HowdyThere" });

            mockGlobalDataAccessor
                .Setup(globalDataAccessor => globalDataAccessor.GetJustInTimeAgentIds(It.IsAny<List<string>>()))
                .ReturnsAsync(new List<AgentDTO>
                {
                    new AgentDTO
                    {
                        AGENT = "654321",
                        AGENT_LEVEL = "13",
                        MARKET_CODE = "IS"
                    }
                });

            var expectedAgentMaster = new PAGNT_AGENT_MASTER();
            mockLifeProAccessor
                .Setup(lifeProAccessor => lifeProAccessor.GetPAGNT_AGENT_MASTER(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(
                    new List<PAGNT_AGENT_MASTER>
                    {
                        expectedAgentMaster
                    });

            mockGlobalDataAccessor
                .Setup(globalDataAccessor => globalDataAccessor.GetAgentFolderIdsFromAttributes(It.IsAny<string>()))
                .ReturnsAsync(new List<string> { "agentFolderId" });

            mockGlobalDataAccessor
                .Setup(globalDataAccessor => globalDataAccessor.GetQueueFromFolderId("agentFolderId"))
                .ReturnsAsync(expectedQueueDescription);

            var expectedJitAgentDto = new JustInTimeAgentDTO
            {
                AgentId = "ABC1"
            };

            mockGlobalDataAccessor
                .Setup(globalDataAccessor => globalDataAccessor.GetJitAgentInfoFromFolderId("654321", "agentFolderId", "IS", "13"))
                .ReturnsAsync(expectedJitAgentDto);

            var expectedJitNameDto = new JustInTimeAgentNameDTO();
            mockGlobalDataAccessor
                .Setup(globalDataAccessor => globalDataAccessor.GetAgentName(expectedJitAgentDto.AgentId))
                .ReturnsAsync(expectedJitNameDto);

            var jitAgentId = "JIT";
            mockPolicyMapper
                .Setup(m => m.MapAgent(expectedJitAgentDto, expectedJitNameDto))
                .Returns(new Agent { AgentId = jitAgentId });

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                mockGlobalDataAccessor.Object,
                mockSupportDataAccessor.Object,
                null,
                null,
                mockLogger.Object,
                mockPolicyMapper.Object,
                null);

            var applicationDate = new DateTime(2022, 01, 01);

            // Act
            var agents = await policyEngine.GetAgents(policyNumber, "01", applicationDate);

            // Assert
            Assert.IsNotNull(agents);
            Assert.AreEqual(2, agents.Count);
            Assert.IsNotNull(agents.SingleOrDefault(a => a.AgentId == jitAgentId));
            Assert.IsNotNull(agents.SingleOrDefault(a => a.AgentId == nonJitAgent.AgentNumber));
        }

        [TestMethod]
        public void GetOwners_ShouldReturn_AddedOwners()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";
            var participantDtos = new List<ParticipantDTO>();
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(m => m.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.OwnerRelateCodes))
                .Returns(participantDtos);

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapOwners(participantDtos))
                .Returns(new List<Owner> { new Owner() });

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                null,
                mockPolicyMapper.Object,
                null);

            // Act
            var addedOwners = policyEngine.GetOwners(policyNumber, companyCode);

            // Assert
            Assert.AreEqual(1, addedOwners.Count);
        }

        [TestMethod]
        public void GetPayee_ShouldReturn_AddedPayee()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";
            var participantDtos = new List<ParticipantDTO>();
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(m => m.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.PayeeRelateCodes))
                .Returns(participantDtos);

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapPayee(participantDtos))
                .Returns(new Payee());

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                null,
                mockPolicyMapper.Object,
                null);

            // Act
            var addedPayee = policyEngine.GetPayee(policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(addedPayee);
        }

        [TestMethod]
        public void GetPayors_ShouldReturn_AddedPayors()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";
            var participantDtos = new List<ParticipantDTO>();
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(m => m.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.PayorRelateCodes))
                .Returns(participantDtos);

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapPayors(participantDtos))
                .Returns(new List<Payor> { new Payor() });

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                null,
                mockPolicyMapper.Object,
                null);

            // Act
            var addedPayors = policyEngine.GetPayors(policyNumber, companyCode);

            // Assert
            Assert.AreEqual(1, addedPayors.Count);
        }

        [TestMethod]
        public void GetAnnuitants_ShouldReturn_AddedAnnuitants()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";
            var participantDtos = new List<ParticipantDTO>();
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(m => m.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.AnnuitantRelateCodes))
                .Returns(participantDtos);

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapAnnuitants(participantDtos))
                .Returns(new List<Annuitant> { new Annuitant() });

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                null,
                mockPolicyMapper.Object,
                null);

            // Act
            var addedAnnuitants = policyEngine.GetAnnuitants(policyNumber, companyCode);

            // Assert
            Assert.AreEqual(1, addedAnnuitants.Count);
        }

        [TestMethod]
        public void GetAssignee_ShouldReturn_AddedAssignee()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";
            var participantDtos = new List<ParticipantDTO>();
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(m => m.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.AssigneeRelateCodes))
                .Returns(participantDtos);

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapAssignee(participantDtos))
                .Returns(new Assignee());

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                null,
                mockPolicyMapper.Object,
                null);

            // Act
            var addedAssignee = policyEngine.GetAssignee(policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(addedAssignee);
        }

        [TestMethod]
        public void GetBeneficiaries_ShouldReturn_AddedBeneficiaries()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";
            var participantDtos = new List<ParticipantDTO>();
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(m => m.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.BeneficiaryRelateCodes))
                .Returns(participantDtos);

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapBeneficiaries(participantDtos))
                .Returns(new List<Beneficiary> { new Beneficiary() });

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                null,
                mockPolicyMapper.Object,
                null);

            // Act
            var addedBeneficiaries = policyEngine.GetBeneficiaries(policyNumber, companyCode);

            // Assert
            Assert.AreEqual(1, addedBeneficiaries.Count);
        }

        [TestMethod]
        public void GetInsureds_ShouldReturn_Insureds()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";
            var participantDtos = new List<ParticipantDTO>();
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(m => m.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.InsuredRelateCodes))
                .Returns(participantDtos);

            var benefitDtos = new List<BenefitDTO>();
            mockLifeProAccessor
                .Setup(m => m.GetBenefitDTOs(policyNumber, companyCode))
                .Returns(benefitDtos);

            var mockPolicyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            mockPolicyMapper
                .Setup(m => m.MapInsureds(participantDtos, benefitDtos))
                .Returns(new List<Insured> { new Insured() });

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                null,
                mockPolicyMapper.Object,
                null);

            // Act
            var insureds = policyEngine.GetInsureds(policyNumber, companyCode);

            // Assert
            Assert.AreEqual(1, insureds.Count);
        }

        [TestMethod]
        public async Task DeletePolicy_ShouldDeletePolicy()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";
            var hierarchy = new AgentHierarchy
            {
                Agent = new Agent
                {
                    AgentId = "ABC1"
                },
                HierarchyAgents = new List<HierarchyAgent>
                {
                    new HierarchyAgent
                    {
                        AgentId = "ABC2"
                    },
                    new HierarchyAgent
                    {
                        AgentId = "ABC3"
                    }
                }
            };

            var mockEventsAccessor = new Mock<IEventsAccessor>();
            mockEventsAccessor
                .Setup(m => m.GetPolicyHierarchyAsync(policyNumber, companyCode))
                .ReturnsAsync(new PolicyHierarchy { HierarchyBranches = new List<AgentHierarchy> { hierarchy } });

            mockEventsAccessor
                .Setup(m => m.RemoveAgentPolicyAccessAsync("ABC1", policyNumber, companyCode))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(m => m.RemoveAgentPolicyAccessAsync("ABC2", policyNumber, companyCode))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(m => m.RemoveAgentPolicyAccessAsync("ABC3", policyNumber, companyCode))
                .ReturnsAsync(1);

            mockEventsAccessor
                .Setup(m => m.DeletePolicyAsync(policyNumber, companyCode))
                .ReturnsAsync(1);

            var consumerPolicyEngine = new ConsumerPolicyEngine(
                null,
                null,
                null,
                mockEventsAccessor.Object,
                null,
                mockLogger.Object,
                null,
                null);

            // Act
            await consumerPolicyEngine.DeletePolicy(policyNumber, companyCode);

            // Assert
            mockEventsAccessor
                .Verify(m => m.RemoveAgentPolicyAccessAsync(It.IsAny<string>(), policyNumber, companyCode), Times.Exactly(3));
        }

        [TestMethod]
        public void GetReturnPaymentData_ShouldReturn_ReturnPaymentType_InitialPaymentCheckDraftDeclined()
        {
            // Arrange
            var mostRecentPaymentDate = 20220125;
            var mostRecentPaymentDatePaymentDto =
                new PaymentDTO
                {
                    EffectiveDate = mostRecentPaymentDate
                };

            var checkDeclinedPaymentDto =
                new PaymentDTO
                {
                    CreditCode = 2,
                    EffectiveDate = mostRecentPaymentDate
                };

            var paymentDtos = new List<PaymentDTO>
            {
                mostRecentPaymentDatePaymentDto,
                checkDeclinedPaymentDto
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(m => m.GetPaymentData(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(paymentDtos);

            mockLifeProAccessor
                .Setup(m => m.IsInitialPaymentDeclined(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(true);

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                mockLogger.Object,
                null,
                null);

            // Act
            var returnPaymentData = policyEngine.GetReturnPaymentData("123456789", "01");

            // Assert
            Assert.AreEqual(ReturnPaymentType.InitialPaymentCheckDraftDeclined, returnPaymentData.returnPaymentType);
            Assert.AreEqual(new DateTime(2022, 01, 25), returnPaymentData.returnPaymentDate);
        }

        [TestMethod]
        public void GetReturnPaymentData_ShouldReturn_ReturnPaymentType_InitialPaymentCardDeclined()
        {
            // Arrange
            var mostRecentPaymentDate = 20220125;
            var mostRecentPaymentDatePaymentDto =
                new PaymentDTO
                {
                    EffectiveDate = mostRecentPaymentDate
                };

            var creditCardDeclinedPaymentDto =
                new PaymentDTO
                {
                    CreditCode = 110,
                    DebitCode = 771,
                    ReversalCode = "Y",
                    EffectiveDate = mostRecentPaymentDate
                };

            var paymentDtos = new List<PaymentDTO>
            {
                mostRecentPaymentDatePaymentDto,
                creditCardDeclinedPaymentDto
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor.Setup(lifeProAccessor => lifeProAccessor.IsInitialPaymentDeclined(
                   It.IsAny<string>(),
                   It.IsAny<string>()))
                .Returns(true);

            mockLifeProAccessor.Setup(lifeProAccessor => lifeProAccessor.GetPaymentData(
                   It.IsAny<string>(),
                   It.IsAny<string>()))
                .Returns(paymentDtos);

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                mockLogger.Object,
                null,
                null);

            // Act
            var returnPaymentData = policyEngine.GetReturnPaymentData("123456789", "01");

            // Assert
            Assert.AreEqual(ReturnPaymentType.InitialPaymentCardDeclined, returnPaymentData.returnPaymentType);
            Assert.AreEqual(new DateTime(2022, 01, 25), returnPaymentData.returnPaymentDate);
        }

        [TestMethod]
        public void GetReturnPaymentData_ShouldReturn_ReturnPaymentType_CardDeclined()
        {
            // Arrange
            var mostRecentPaymentDate = 20220125;
            var mostRecentPaymentDatePaymentDto =
                new PaymentDTO
                {
                    EffectiveDate = mostRecentPaymentDate
                };

            var creditCardDeclinedPaymentDto =
                new PaymentDTO
                {
                    CreditCode = 110,
                    DebitCode = 771,
                    ReversalCode = "Y",
                    EffectiveDate = mostRecentPaymentDate
                };

            var paymentDtos = new List<PaymentDTO>
            {
                mostRecentPaymentDatePaymentDto,
                creditCardDeclinedPaymentDto
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor.Setup(lifeProAccessor => lifeProAccessor.IsInitialPaymentDeclined(
                   It.IsAny<string>(),
                   It.IsAny<string>()))
                .Returns(false);

            mockLifeProAccessor.Setup(lifeProAccessor => lifeProAccessor.GetPaymentData(
                   It.IsAny<string>(),
                   It.IsAny<string>()))
                .Returns(paymentDtos);

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                mockLogger.Object,
                null,
                null);

            // Act
            var returnPaymentData = policyEngine.GetReturnPaymentData("123456789", "01");

            // Assert
            Assert.AreEqual(ReturnPaymentType.CardDeclined, returnPaymentData.returnPaymentType);
            Assert.AreEqual(new DateTime(2022, 01, 25), returnPaymentData.returnPaymentDate);
        }

        [TestMethod]
        public void GetReturnPaymentData_ShouldReturn_ReturnPaymentType_CheckDraftDeclined()
        {
            // Arrange
            var mostRecentPaymentDate = 20220125;
            var mostRecentPaymentDatePaymentDto =
                new PaymentDTO
                {
                    EffectiveDate = mostRecentPaymentDate
                };

            var checkDeclinedPaymentDto =
                new PaymentDTO
                {
                    CreditCode = 2,
                    EffectiveDate = mostRecentPaymentDate
                };

            var paymentDtos = new List<PaymentDTO>
            {
                mostRecentPaymentDatePaymentDto,
                checkDeclinedPaymentDto
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(m => m.GetPaymentData(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(paymentDtos);

            mockLifeProAccessor
                .Setup(m => m.IsInitialPaymentDeclined(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(false);

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                mockLogger.Object,
                null,
                null);

            // Act
            var returnPaymentData = policyEngine.GetReturnPaymentData("123456789", "01");

            // Assert
            Assert.AreEqual(ReturnPaymentType.CheckDraftDeclined, returnPaymentData.returnPaymentType);
            Assert.AreEqual(new DateTime(2022, 01, 25), returnPaymentData.returnPaymentDate);
        }

        [TestMethod]
        public void GetReturnPaymentData_ShouldReturn_ReturnPaymentType_None()
        {
            // Arrange
            var mostRecentPaymentDate = 20220125;
            var mostRecentPaymentDatePaymentDto =
                new PaymentDTO
                {
                    EffectiveDate = mostRecentPaymentDate
                };

            var paymentDtos = new List<PaymentDTO>
            {
                mostRecentPaymentDatePaymentDto
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(m => m.GetPaymentData(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(paymentDtos);

            mockLifeProAccessor
                .Setup(m => m.IsInitialPaymentDeclined(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(false);

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                mockLogger.Object,
                null,
                null);

            // Act
            var returnPaymentData = policyEngine.GetReturnPaymentData("123456789", "01");

            // Assert
            Assert.AreEqual(ReturnPaymentType.None, returnPaymentData.returnPaymentType);
            Assert.IsNull(returnPaymentData.returnPaymentDate);
        }

        [TestMethod]
        public void GetReturnPaymentData_GetMostRecentPaymentDate_Null_ShouldReturn_ReturnPaymentType_None()
        {
            // Arrange
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(m => m.GetPaymentData(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Returns(null as List<PaymentDTO>);

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                mockLogger.Object,
                null,
                null);

            // Act
            var returnPaymentData = policyEngine.GetReturnPaymentData("123456789", "01");

            // Assert
            Assert.AreEqual(ReturnPaymentType.None, returnPaymentData.returnPaymentType);
            Assert.IsNull(returnPaymentData.returnPaymentDate);
        }

        [TestMethod]
        public async Task GetEmployer_ShouldReturnEmployer()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";
            var employerDto = new EmployerDTO();
            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(m => m.GetEmployerDetail(policyNumber, companyCode))
                .ReturnsAsync(employerDto);

            var policyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            policyMapper
                .Setup(m => m.MapEmployer(employerDto))
                .Returns(new Employer());

            var policyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                mockLogger.Object,
                policyMapper.Object,
                null);

            // Act
            var employer = await policyEngine.GetEmployer(policyNumber, companyCode);

            // Assert
            Assert.IsNotNull(employer);
        }

        [TestMethod]
        public void GetBenefit_ShouldReturnBenefit()
        {
            // Arrange
            var policyNumber = "123456789";
            var companyCode = "01";
            var companyCodeAndPolicyNumber = new CompanyCodeAndPolicyNumber(companyCode, policyNumber);
            long pbenId = 5;
            var lineOfBusiness = LineOfBusiness.UniversalLife;
            var id1MaxExtendedKeys = "020202";
            var id2ExtendedKeys = "0303";
            var benefitDto = new BenefitDTO
            {
                MultipleInsureds = new List<MultipleInsuredDTO>
                {
                    new MultipleInsuredDTO
                    {
                        KdDefSegmentId = "id1",
                        KdBenefitExtendedKeys = "0101",
                    },
                    new MultipleInsuredDTO
                    {
                        KdDefSegmentId = "id1",
                        KdBenefitExtendedKeys = id1MaxExtendedKeys
                    },
                    new MultipleInsuredDTO
                    {
                        KdDefSegmentId = "id3",
                        KdBenefitExtendedKeys = id2ExtendedKeys,
                    }
                }
            };

            var expectedLookup1 =
                new KeyLookup
                {
                    Identifier = "id1",
                    MaxKeyValue = 2,
                    MaxOrdinal = (short)(id1MaxExtendedKeys.Length / 2)
                };

            var expectedLookup2 =
                new KeyLookup
                {
                    Identifier = "id3",
                    MaxKeyValue = 3,
                    MaxOrdinal = (short)(id2ExtendedKeys.Length / 2)
                };

            var expectedExtendedKeysLookup = new ExtendedKeysLookup
            {
                Lookups = new List<KeyLookup>
                {
                }
            };

            var mockLifeProAccessor = new Mock<ILifeProAccessor>(MockBehavior.Strict);
            mockLifeProAccessor
                .Setup(m => m.GetBenefitDTO(policyNumber, companyCode, pbenId))
                .Returns(benefitDto);

            var keyLookupResult = new List<ExtendedKeyLookupResult>();
            mockLifeProAccessor
                .Setup(m => m.GetExtendedKeyData(It.Is<ExtendedKeysLookup>(
                    lookup =>
                        lookup.Lookups.Contains(expectedLookup1)
                        && lookup.Lookups.Contains(expectedLookup2))))
                .Returns(keyLookupResult);

            var policyMapper = new Mock<IPolicyMapper>(MockBehavior.Strict);
            policyMapper
                .Setup(m => m.MapBenefit(lineOfBusiness, benefitDto, keyLookupResult))
                .Returns(new Benefit());

            var consumerPolicyEngine = new ConsumerPolicyEngine(
                mockLifeProAccessor.Object,
                null,
                null,
                null,
                null,
                mockLogger.Object,
                policyMapper.Object,
                null);

            // Act
            var benefit = consumerPolicyEngine.GetBenefit(
                companyCodeAndPolicyNumber,
                lineOfBusiness,
                new PPBEN_POLICY_BENEFITS { PBEN_ID = pbenId });

            // Assert
            Assert.IsNotNull(benefit);
        }
    }
}