namespace Assurity.Kafka.Engines.Tests.Mapping
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Requirements;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Mapping;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RequirementsMapperTests
    {
        [TestMethod]
        public void GenerateSyntheticRequirement_ShouldReturnRequirement()
        {
            // Arrange
            var requirementsMapper = new RequirementsMapper(null);

            // Act
            var result = requirementsMapper.GenerateHomeOfficeReviewRequirement();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.AddedDate);
            Assert.AreEqual(RequirementStatus.Unmet, result.Status);
            Assert.AreEqual("Pending Home Office Review", result.Name);
            Assert.AreEqual(0, result.Id);
            Assert.AreEqual(RequirementFulfillingParty.HomeOffice, result.FulfillingParty);
            Assert.IsTrue(result.Display);
        }

        [TestMethod]
        public void MapRequirements_ShouldReturnRequirement()
        {
            // Arrange
            var unexpectedRequirement = new PolicyRequirement
            {
                Id = 999
            };

            var requirementId1 = 5;
            var requirementId2 = 7;
            var nameId1 = 151515;
            var requirementType1 = "3RDPTY";
            var expectedRequirement1 = new PolicyRequirement
            {
                AddedDate = 19990101,
                ObtainedDate = 20020505,
                Status = "W",
                Description = "desc1",
                Id = (short)requirementId1,
                LifeproComment = "Yup",
                NameId = nameId1,
                ReqType = requirementType1
            };

            var expectedRequirement2 = new PolicyRequirement
            {
                AddedDate = 19990909,
                ObtainedDate = 20030808,
                Status = "Y",
                Description = "desc2",
                Id = (short)requirementId2,
                LifeproComment = "Nope",
                NameId = 999999
            };

            var policyRequirements = new List<PolicyRequirement>
            {
                unexpectedRequirement,
                expectedRequirement1,
                expectedRequirement2
            };

            var requirementMapping1 = new RequirementMapping
            {
                RequirementId = requirementId1,
                Phone = "Phone1",
                FulfillingParty = "Home Office",
                AgentAction = null,
                Display = false
            };

            var requirementMapping2 = new RequirementMapping
            {
                RequirementId = requirementId2,
                Phone = "Phone2",
                FulfillingParty = "Agent",
                AgentAction = "SendMessage",
                Display = true
            };

            var requirementMappings = new List<RequirementMapping>
            {
                requirementMapping1,
                requirementMapping2
            };

            var expectedParticipant = new ParticipantDTO
            {
                Name = new NameDTO { NameId = nameId1 }
            };

            var participants = new List<ParticipantDTO>
            {
                expectedParticipant
            };

            var globalCommentLookupResult = new GlobalRequirementLookupResult
            {
                Ix = expectedRequirement1.Ix,
                Sequence = expectedRequirement1.ReqSequence,
                Type = expectedRequirement1.ReqType,
                Note = "note"
            };

            var globalCommentData = new List<GlobalRequirementLookupResult>
            {
                globalCommentLookupResult
            };

            var participantMapper = new Mock<IParticipantMapper>();
            participantMapper
                .Setup(m => m.MapParticipant(expectedParticipant))
                .Returns(new PolicyInfo.Contracts.V1.Participant());

            var requirementsMapper = new RequirementsMapper(participantMapper.Object);

            // Act
            var result = requirementsMapper.MapRequirements(
                policyRequirements,
                requirementMappings,
                participants,
                globalCommentData);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);

            var mappedRequirement1 = result.Single(r => r.LifeProComment == expectedRequirement1.LifeproComment);
            Assert.AreEqual(expectedRequirement1.Status.ToRequirementStatus(), mappedRequirement1.Status);
            Assert.AreEqual(expectedRequirement1.ObtainedDate.ToNullableDateTime(), mappedRequirement1.ObtainedDate);
            Assert.AreEqual(expectedRequirement1.AddedDate.ToNullableDateTime(), mappedRequirement1.AddedDate);
            Assert.AreEqual(expectedRequirement1.Description, mappedRequirement1.Name);
            Assert.AreEqual(expectedRequirement1.Id, mappedRequirement1.Id);
            Assert.AreEqual(expectedRequirement1.LifeproComment, mappedRequirement1.LifeProComment);

            Assert.AreEqual(globalCommentLookupResult.Note, mappedRequirement1.GlobalComment);

            Assert.AreEqual(requirementMapping1.Phone, mappedRequirement1.PhoneNumberComment);
            Assert.AreEqual(requirementMapping1.FulfillingParty.ToRequirementFulfillingParty(), mappedRequirement1.FulfillingParty);
            Assert.IsNull(mappedRequirement1.ActionType);
            Assert.AreEqual(requirementMapping1.Phone, mappedRequirement1.PhoneNumberComment);
            Assert.AreEqual(requirementMapping1.Display, mappedRequirement1.Display);
            Assert.IsNotNull(mappedRequirement1.AppliesTo);

            var mappedRequirement2 = result.Single(r => r.LifeProComment == expectedRequirement2.LifeproComment);
            Assert.AreEqual(expectedRequirement2.Status.ToRequirementStatus(), mappedRequirement2.Status);
            Assert.AreEqual(expectedRequirement2.ObtainedDate.ToNullableDateTime(), mappedRequirement2.ObtainedDate);
            Assert.AreEqual(expectedRequirement2.AddedDate.ToNullableDateTime(), mappedRequirement2.AddedDate);
            Assert.AreEqual(expectedRequirement2.Description, mappedRequirement2.Name);
            Assert.AreEqual(expectedRequirement2.Id, mappedRequirement2.Id);
            Assert.AreEqual(expectedRequirement2.LifeproComment, mappedRequirement2.LifeProComment);

            Assert.IsNull(mappedRequirement2.GlobalComment);

            Assert.AreEqual(requirementMapping2.Phone, mappedRequirement2.PhoneNumberComment);
            Assert.AreEqual(requirementMapping2.FulfillingParty.ToRequirementFulfillingParty(), mappedRequirement2.FulfillingParty);
            Assert.AreEqual(requirementMapping2.AgentAction.ToRequirementActionType(), mappedRequirement2.ActionType);
            Assert.AreEqual(requirementMapping2.Phone, mappedRequirement2.PhoneNumberComment);
            Assert.AreEqual(requirementMapping2.Display, mappedRequirement2.Display);

            Assert.IsNull(mappedRequirement2.AppliesTo);
        }
    }
}
