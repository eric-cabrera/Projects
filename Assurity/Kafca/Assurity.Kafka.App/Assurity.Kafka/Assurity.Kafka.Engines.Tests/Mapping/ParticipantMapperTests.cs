namespace Assurity.Kafka.Engines.Tests.Mapping
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Mapping;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ParticipantMapperTests
    {
        private ParticipantMapper Mapper => new ParticipantMapper();

        [TestMethod]
        public void MapParticipant_NotBusiness_ShouldMap()
        {
            // Arrange
            var testParticipant = GetIndividualParticipantDTO();

            // Act
            var mappedParticipant = Mapper.MapParticipant(testParticipant);

            // Assert
            VerifyIndividualParticipantsIdentical(mappedParticipant, testParticipant);
        }

        [TestMethod]
        public void MapParticipant_IsBusiness_ShouldMap()
        {
            // Arrange
            var testParticipant = new ParticipantDTO
            {
                Addresses = new List<AddressDTO>
                {
                    new AddressDTO
                    {
                        AddressId = 1,
                        Line1 = "Line1",
                        Line2 = "Line2",
                        Line3 = "Line3",
                        City = "Omaha",
                        State = "NE",
                        Zip = "68102",
                        ZipExtension = "1234",
                        BoxNumber = "Box1",
                        Country = "USA",
                        TelephoneNumber = "4025551234"
                    }
                },
                Name = new NameDTO
                {
                    NameFormatCode = "B",
                    BusinessEmailAdress = "jusMyLilBiz@biz.com",
                    NameBusiness = "Dance and Bake",
                    NameId = 1,
                },
                SexCode = "M",
                DateOfBirth = 19800101
            };

            // Act
            var mappedParticipant = Mapper.MapParticipant(testParticipant);

            // Assert
            Assert.AreEqual(testParticipant.Name.BusinessEmailAdress, mappedParticipant.Business.EmailAddress);
            Assert.AreEqual(testParticipant.Name.NameBusiness, mappedParticipant.Business.Name.BusinessName);
            Assert.IsTrue(mappedParticipant.IsBusiness);
            Assert.AreEqual(testParticipant.Name.NameId, mappedParticipant.Business.Name.NameId);

            var expectedAddress = testParticipant.Addresses.First();
            Assert.AreEqual(expectedAddress.AddressId, mappedParticipant.Address.AddressId);
            Assert.AreEqual(expectedAddress.Line1, mappedParticipant.Address.Line1);
            Assert.AreEqual(expectedAddress.Line2, mappedParticipant.Address.Line2);
            Assert.AreEqual(expectedAddress.Line3, mappedParticipant.Address.Line3);
            Assert.AreEqual(expectedAddress.City, mappedParticipant.Address.City);
            Assert.AreEqual(expectedAddress.State, mappedParticipant.Address.StateAbbreviation.ToString());
            Assert.AreEqual(expectedAddress.Zip, mappedParticipant.Address.ZipCode);
            Assert.AreEqual(expectedAddress.ZipExtension, mappedParticipant.Address.ZipExtension);
            Assert.AreEqual(expectedAddress.BoxNumber, mappedParticipant.Address.BoxNumber);
            Assert.AreEqual(expectedAddress.Country, mappedParticipant.Address.Country.ToString());
            Assert.AreEqual(expectedAddress.TelephoneNumber, mappedParticipant.PhoneNumber);
        }

        [TestMethod]
        public void MapInsureds_NoParticipantsWithInsureRelateCodes_ShouldReturnNull()
        {
            // Arrange
            var participants = new List<ParticipantDTO>
            {
                new ParticipantDTO
                {
                    RelateCode = RelateCodes.PayeeRelateCodes.First()
                }
            };

            // Act
            var insureds = Mapper.MapInsureds(participants, null);

            // Assert
            Assert.IsNull(insureds);
        }

        [TestMethod]
        public void MapInsureds_NoMatchingBenefitSequenceNumber_ShouldReturnEmptyList()
        {
            // Arrange
            var participants = new List<ParticipantDTO>
            {
                GetIndividualParticipantDTO()
            };

            var benefitDtos = new List<BenefitDTO>
            {
                new BenefitDTO
                {
                    BenefitSequence = 2
                }
            };

            // Act
            var mappedInsureds = Mapper.MapInsureds(participants, benefitDtos);

            // Assert
            Assert.IsNotNull(mappedInsureds);
            Assert.AreEqual(0, mappedInsureds.Count);
        }

        [TestMethod]
        public void MapInsureds_MatchingBenefitSequenceNumber_MultipleParticipantsWithIdenticalNameIds_ShouldOnlyMapOnce()
        {
            // Arrange
            short benefitSequence = 1;
            var participants = new List<ParticipantDTO>
            {
                GetIndividualParticipantDTO(),
                GetIndividualParticipantDTO()
            };

            var benefitDtos = new List<BenefitDTO>
            {
                new BenefitDTO
                {
                    BenefitSequence = benefitSequence
                }
            };

            // Act
            var insureds = Mapper.MapInsureds(participants, benefitDtos);

            // Assert
            var participant = participants.First();
            var mappedInsured = insureds.Single();
            Assert.AreEqual(RelationshipToPrimaryInsured.Self, mappedInsured.RelationshipToPrimaryInsured);
            Assert.AreEqual(participant.Name.IndividualFirst, mappedInsured.Participant.Person.Name.IndividualFirst);
            Assert.AreEqual(participant.Name.IndividualLast, mappedInsured.Participant.Person.Name.IndividualLast);
            Assert.AreEqual(participant.Name.IndividualMiddle, mappedInsured.Participant.Person.Name.IndividualMiddle);
            Assert.AreEqual(participant.Name.IndividualSuffix, mappedInsured.Participant.Person.Name.IndividualSuffix);
            Assert.AreEqual(participant.Name.NameId, mappedInsured.Participant.Person.Name.NameId);

            var expectedAddress = participant.Addresses.First();
            Assert.AreEqual(expectedAddress.AddressId, mappedInsured.Participant.Address.AddressId);
            Assert.AreEqual(expectedAddress.Line1, mappedInsured.Participant.Address.Line1);
            Assert.AreEqual(expectedAddress.Line2, mappedInsured.Participant.Address.Line2);
            Assert.AreEqual(expectedAddress.Line3, mappedInsured.Participant.Address.Line3);
            Assert.AreEqual(expectedAddress.City, mappedInsured.Participant.Address.City);
            Assert.AreEqual(expectedAddress.State, mappedInsured.Participant.Address.StateAbbreviation.ToString());
            Assert.AreEqual(expectedAddress.Zip, mappedInsured.Participant.Address.ZipCode);
            Assert.AreEqual(expectedAddress.ZipExtension, mappedInsured.Participant.Address.ZipExtension);
            Assert.AreEqual(expectedAddress.BoxNumber, mappedInsured.Participant.Address.BoxNumber);
            Assert.AreEqual(expectedAddress.Country, mappedInsured.Participant.Address.Country.ToString());
            Assert.AreEqual(expectedAddress.TelephoneNumber, mappedInsured.Participant.PhoneNumber);

            Assert.AreEqual(Gender.Male, mappedInsured.Participant.Person.Gender);
            Assert.AreEqual(participant.DateOfBirth.ToNullableDateTime(), mappedInsured.Participant.Person.DateOfBirth);

            Assert.IsFalse(mappedInsured.Participant.IsBusiness);
        }

        [TestMethod]
        public void MapInsureds_RelateCodeIN_BenefitSeq1ShouldBeSelf_Not1ShouldBeAdditional()
        {
            // Arrange
            var benefitSequence1 = GetIndividualParticipantDTO();
            benefitSequence1.BenefitSequenceNumber = 1;
            benefitSequence1.Name.IndividualFirst = "Self";

            var benefitSequence2 = GetIndividualParticipantDTO();
            benefitSequence2.BenefitSequenceNumber = 2;
            benefitSequence2.Name.IndividualFirst = "Additional";
            benefitSequence2.Name.NameId = 5;
            var participants = new List<ParticipantDTO>
            {
                benefitSequence1,
                benefitSequence2
            };

            var benefitDtos = new List<BenefitDTO>
            {
                new BenefitDTO
                {
                    BenefitSequence = 1,
                },
                new BenefitDTO
                {
                    BenefitSequence = 2,
                }
            };

            // Act
            var insureds = Mapper.MapInsureds(participants, benefitDtos);

            // Assert
            var selfInsured = insureds.SingleOrDefault(i => i.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Self);
            Assert.IsNotNull(selfInsured);
            Assert.AreEqual(benefitSequence1.Name.IndividualFirst, selfInsured.Participant.Person.Name.IndividualFirst);

            var additionalInsured = insureds.SingleOrDefault(i => i.RelationshipToPrimaryInsured == RelationshipToPrimaryInsured.Additional);
            Assert.IsNotNull(additionalInsured);
            Assert.AreEqual(benefitSequence2.Name.IndividualFirst, additionalInsured.Participant.Person.Name.IndividualFirst);
        }

        [TestMethod]
        public void MapInsureds_NoBenefit_InsuredTypeJE_ShouldMapInsured()
        {
            // Arrange
            short benefitSequence = 1;
            var participant = GetIndividualParticipantDTO();
            participant.BenefitSequenceNumber = benefitSequence;
            participant.RelateCode = "JE";
            var participants = new List<ParticipantDTO> { participant };

            // Act
            var insureds = Mapper.MapInsureds(participants, null);

            // Assert
            var mappedInsured = insureds.Single();
            Assert.AreEqual(RelationshipToPrimaryInsured.Joint, mappedInsured.RelationshipToPrimaryInsured);
            VerifyIndividualParticipantsIdentical(mappedInsured.Participant, participant);
        }

        [TestMethod]
        public void MapInsureds_RelateCodeML_BenefitStatusCodeT_BenefitStatusDateLessThanPMuinStartDate_ShouldNotMap()
        {
            // Arrange
            short benefitSequence = 1;
            var participants = new List<ParticipantDTO>
            {
                new ParticipantDTO
                {
                    BenefitSequenceNumber = benefitSequence,
                    Name = new NameDTO { NameId = 5 },
                    RelateCode = "ML"
                }
            };

            var benefitDtos = new List<BenefitDTO>
            {
                new BenefitDTO
                {
                    BenefitSequence = benefitSequence,
                    StatusCode = "T",
                    StatusDate = 0,
                    MultipleInsureds = new List<MultipleInsuredDTO>
                    {
                        new MultipleInsuredDTO
                        {
                            StartDate = 1
                        }
                    }
                }
            };

            // Act
            var insureds = Mapper.MapInsureds(participants, benefitDtos);

            // Assert
            Assert.AreEqual(0, insureds.Count);
        }

        [TestMethod]
        public void MapInsureds_RelateCodeML_BenefitStatusCodeT_BenefitStatusDateGreaterThanPMuinEndDate_ShouldNotMap()
        {
            // Arrange
            short benefitSequence = 1;
            var participants = new List<ParticipantDTO>
            {
                new ParticipantDTO
                {
                    BenefitSequenceNumber = benefitSequence,
                    Name = new NameDTO { NameId = 5 },
                    RelateCode = "ML"
                }
            };

            var benefitDtos = new List<BenefitDTO>
            {
                new BenefitDTO
                {
                    BenefitSequence = benefitSequence,
                    StatusCode = "T",
                    StatusDate = 1,
                    MultipleInsureds = new List<MultipleInsuredDTO>
                    {
                        new MultipleInsuredDTO
                        {
                            StopDate = 0
                        }
                    }
                }
            };

            // Act
            var insureds = Mapper.MapInsureds(participants, benefitDtos);

            // Assert
            Assert.AreEqual(0, insureds.Count);
        }

        [TestMethod]
        public void MapInsureds_RelateCodeML_BenefitStatusCodeA_MultipleInsuredStopDateLessThanToday_ShouldNotMap()
        {
            // Arrange
            short benefitSequence = 1;
            var participants = new List<ParticipantDTO>
            {
                new ParticipantDTO
                {
                    BenefitSequenceNumber = benefitSequence,
                    Name = new NameDTO { NameId = 5 },
                    RelateCode = "ML"
                }
            };

            var benefitDtos = new List<BenefitDTO>
            {
                new BenefitDTO
                {
                    BenefitSequence = benefitSequence,
                    StatusCode = "A",
                    MultipleInsureds = new List<MultipleInsuredDTO>
                    {
                        new MultipleInsuredDTO
                        {
                            StopDate = 0
                        }
                    }
                }
            };

            // Act
            var insureds = Mapper.MapInsureds(participants, benefitDtos);

            // Assert
            Assert.AreEqual(0, insureds.Count);
        }

        [TestMethod]
        public void MapInsureds_RelateCodeML_BenefitStatusCodeP_MultipleInsuredStopDateLessThanToday_ShouldNotMap()
        {
            // Arrange
            short benefitSequence = 1;
            var participants = new List<ParticipantDTO>
            {
                new ParticipantDTO
                {
                    BenefitSequenceNumber = benefitSequence,
                    Name = new NameDTO { NameId = 5 },
                    RelateCode = "ML"
                }
            };

            var benefitDtos = new List<BenefitDTO>
            {
                new BenefitDTO
                {
                    BenefitSequence = benefitSequence,
                    StatusCode = "P",
                    MultipleInsureds = new List<MultipleInsuredDTO>
                    {
                        new MultipleInsuredDTO
                        {
                            StopDate = 0
                        }
                    }
                }
            };

            // Act
            var insureds = Mapper.MapInsureds(participants, benefitDtos);

            // Assert
            Assert.AreEqual(0, insureds.Count);
        }

        [TestMethod]
        public void MapInsureds_RelateCodeML_BenefitStatusCodeP_MultipleInsuredStopDateGreaterThanToday_ShouldMap()
        {
            // Arrange
            short benefitSequence = 1;
            var testParticipant = GetIndividualParticipantDTO();
            testParticipant.RelateCode = "ML";
            testParticipant.BenefitSequenceNumber = benefitSequence;
            var participants = new List<ParticipantDTO> { testParticipant };

            var benefitDtos = new List<BenefitDTO>
            {
                new BenefitDTO
                {
                    BenefitSequence = benefitSequence,
                    StatusCode = "P",
                    MultipleInsureds = new List<MultipleInsuredDTO>
                    {
                        new MultipleInsuredDTO
                        {
                            NameId = testParticipant.Name.NameId,
                            RelationshipToPrimaryInsured = "BROTHER",
                            StopDate = 99999999
                        }
                    }
                }
            };

            // Act
            var insureds = Mapper.MapInsureds(participants, benefitDtos);

            // Assert
            Assert.AreEqual(1, insureds.Count);
            var mappedInsured = insureds.Single();
            Assert.AreEqual(
                benefitDtos.First().MultipleInsureds.First().RelationshipToPrimaryInsured.ToRelationshipToPrimaryInsured(),
                mappedInsured.RelationshipToPrimaryInsured);

            VerifyIndividualParticipantsIdentical(mappedInsured.Participant, testParticipant);
        }

        [TestMethod]
        public void MapInsureds_RelateCodeML_BenefitStatusCodeA_MultipleInsuredStopDateGreaterThanToday_ShouldMap()
        {
            // Arrange
            short benefitSequence = 1;
            var testParticipant = GetIndividualParticipantDTO();
            testParticipant.RelateCode = "ML";
            testParticipant.BenefitSequenceNumber = benefitSequence;
            var participants = new List<ParticipantDTO> { testParticipant };

            var benefitDtos = new List<BenefitDTO>
            {
                new BenefitDTO
                {
                    BenefitSequence = benefitSequence,
                    StatusCode = "A",
                    MultipleInsureds = new List<MultipleInsuredDTO>
                    {
                        new MultipleInsuredDTO
                        {
                            NameId = testParticipant.Name.NameId,
                            RelationshipToPrimaryInsured = "BROTHER",
                            StopDate = 99999999
                        }
                    }
                }
            };

            // Act
            var insureds = Mapper.MapInsureds(participants, benefitDtos);

            // Assert
            Assert.AreEqual(1, insureds.Count);
            var mappedInsured = insureds.Single();
            Assert.AreEqual(
                benefitDtos.First().MultipleInsureds.First().RelationshipToPrimaryInsured.ToRelationshipToPrimaryInsured(),
                mappedInsured.RelationshipToPrimaryInsured);

            VerifyIndividualParticipantsIdentical(mappedInsured.Participant, testParticipant);
        }

        [TestMethod]
        public void MapInsureds_RelateCodeML_BenefitStatusCodeT_BenefitStatusDateGreaterThanMultipleInsuredStartDateLessThanStopDate()
        {
            // Arrange
            short benefitSequence = 1;
            var testParticipant = GetIndividualParticipantDTO();
            testParticipant.RelateCode = "ML";
            testParticipant.BenefitSequenceNumber = benefitSequence;
            var participants = new List<ParticipantDTO> { testParticipant };

            var benefitDtos = new List<BenefitDTO>
            {
                new BenefitDTO
                {
                    BenefitSequence = benefitSequence,
                    StatusCode = "P",
                    StatusDate = 20220101,
                    MultipleInsureds = new List<MultipleInsuredDTO>
                    {
                        new MultipleInsuredDTO
                        {
                            NameId = testParticipant.Name.NameId,
                            RelationshipToPrimaryInsured = "BROTHER",
                            StartDate = 0,
                            StopDate = 99999999
                        }
                    }
                }
            };

            // Act
            var insureds = Mapper.MapInsureds(participants, benefitDtos);

            // Assert
            Assert.AreEqual(1, insureds.Count);
            var mappedInsured = insureds.Single();
            Assert.AreEqual(
                benefitDtos.First().MultipleInsureds.First().RelationshipToPrimaryInsured.ToRelationshipToPrimaryInsured(),
                mappedInsured.RelationshipToPrimaryInsured);

            VerifyIndividualParticipantsIdentical(mappedInsured.Participant, testParticipant);
        }

        /// <summary>
        /// Stages test data using multiple insured data from DMProd.
        /// </summary>
        /// <remarks>
        /// The data that comes back from GetParticipants contains massive duplication on every field except for benefit sequence.
        /// Differing benefit sequence values account for the duplication.
        /// TODO: The accessor and mapping might be optimized based on this fact.
        /// There's a test coverage hole with respect to this data, so I am embedding sanitized JSON from Prod
        /// for policy number 5051461911. All of the PII has been removed or altered, down to the database ID values.
        /// This data is just too tedious to stage manually.
        /// Expected Results:
        /// - Deceased spouse is not mapped (former primary insured).
        /// - Four children are mapped.
        /// - Surviving spouse is mapped as "Self" relationship to primary insured.
        /// </remarks>
        [TestMethod]
        public void MapInsureds_MultipleRelateCodes_MultipleInsureds_ShouldMap()
        {
            // Arrange
            var currentPath = System.Environment.CurrentDirectory;
            var deceasedSpouseNameId = 3567889;
            var participantsJsonPath = Path.Combine(currentPath, "Mapping", "JsonExamples", "participants.json");
            var benefitsJsonPath = Path.Combine(currentPath, "Mapping", "JsonExamples", "benefits.json");
            var participantsText = File.ReadAllText(participantsJsonPath);
            var benefitsText = File.ReadAllText(benefitsJsonPath);
            var participantData = JsonSerializer.Deserialize<List<ParticipantDTO>>(participantsText);
            var benefitData = JsonSerializer.Deserialize<List<BenefitDTO>>(benefitsText);

            var nameIdsToMap =
                participantData
                .Where(p => p.Name.NameId != deceasedSpouseNameId)
                .Select(p => p.Name.NameId)
                .Distinct()
                .ToList();

            // Act
            var insureds = Mapper.MapInsureds(participantData, benefitData);

            // Assert
            Assert.AreEqual(nameIdsToMap.Count, insureds.Count);
            foreach (var nameId in nameIdsToMap)
            {
                var participantUnderTest = participantData.First(p => p.Name.NameId == nameId);
                var mappedInsured = insureds.Single(i => i.Participant.Person.Name.NameId == nameId);
                VerifyIndividualParticipantsIdentical(mappedInsured.Participant, participantUnderTest);
            }
        }

        [TestMethod]
        public void MapAnnuitants_ShouldMap()
        {
            // Arrange
            var relateCodes = RelateCodes.AnnuitantRelateCodes;
            var participantDTOs = GetParticipantDTOsForParticipantMappingTests(relateCodes);

            // Act
            var mappedParticipants = Mapper.MapAnnuitants(participantDTOs);

            // Assert
            Assert.AreEqual(relateCodes.Count, mappedParticipants.Count);
            for (var i = 0; i < relateCodes.Count; i++)
            {
                var participantUnderTest = participantDTOs.Single(p => p?.Name?.NameId == i);
                var mappedParticipant = mappedParticipants.Single(p => p.Participant.Person.Name.NameId == i);

                Assert.AreEqual(participantUnderTest.RelateCode.ToAnnuitantType(), mappedParticipant.AnnuitantType);
                VerifyIndividualParticipantsIdentical(mappedParticipant.Participant, participantUnderTest);
            }
        }

        [TestMethod]
        public void MapAnnuitants_NoMatchingDTOs_ShouldReturnNull()
        {
            // Arrange
            var relateCodes = RelateCodes.AnnuitantRelateCodes;
            var participantDTOs = GenerateUnpopulatedParticipantWithRelateCode(relateCodes);

            // Act
            var mappedParticipants = Mapper.MapAnnuitants(participantDTOs);

            // Assert
            Assert.IsNull(mappedParticipants);
        }

        [TestMethod]
        public void MapAssignee_ShouldMap()
        {
            // Arrange
            var relateCodes = RelateCodes.AssigneeRelateCodes;
            var participantDTOs = GetParticipantDTOsForParticipantMappingTests(relateCodes);

            // Act
            var mappedParticipant = Mapper.MapAssignee(participantDTOs);

            // Assert
            var participantUnderTest = participantDTOs.Single(p => p?.Name?.NameId == 0);

            VerifyIndividualParticipantsIdentical(mappedParticipant.Participant, participantUnderTest);
        }

        [TestMethod]
        public void MapAssignee_NoMatchingDTOs_ShouldReturnNull()
        {
            // Arrange
            var relateCodes = RelateCodes.AssigneeRelateCodes;
            var participantDTOs = GenerateUnpopulatedParticipantWithRelateCode(relateCodes);

            // Act
            var mappedParticipants = Mapper.MapAssignee(participantDTOs);

            // Assert
            Assert.IsNull(mappedParticipants);
        }

        [TestMethod]
        public void MapBeneficiaries_ShouldMap()
        {
            // Arrange
            var relateCodes = RelateCodes.BeneficiaryRelateCodes;
            var participantDTOs = GetParticipantDTOsForParticipantMappingTests(relateCodes);

            // Act
            var mappedParticipants = Mapper.MapBeneficiaries(participantDTOs);

            // Assert
            Assert.AreEqual(relateCodes.Count, mappedParticipants.Count);
            for (var i = 0; i < relateCodes.Count; i++)
            {
                var participantUnderTest = participantDTOs.Single(p => p?.Name?.NameId == i);
                var mappedParticipant = mappedParticipants.Single(p => p.Participant.Person.Name.NameId == i);

                Assert.AreEqual(participantUnderTest.RelateCode.ToBeneficiaryType(), mappedParticipant.BeneficiaryType);
                VerifyIndividualParticipantsIdentical(mappedParticipant.Participant, participantUnderTest);
            }
        }

        [TestMethod]
        public void MapBeneficiaries_NoMatchingDTOs_ShouldReturnNull()
        {
            // Arrange
            var relateCodes = RelateCodes.BeneficiaryRelateCodes;
            var participantDTOs = GenerateUnpopulatedParticipantWithRelateCode(relateCodes);

            // Act
            var mappedParticipants = Mapper.MapBeneficiaries(participantDTOs);

            // Assert
            Assert.IsNull(mappedParticipants);
        }

        [TestMethod]
        public void MapOwners_ShouldMap()
        {
            // Arrange
            var relateCodes = RelateCodes.OwnerRelateCodes;
            var participantDTOs = GetParticipantDTOsForParticipantMappingTests(relateCodes);

            // Act
            var mappedParticipants = Mapper.MapOwners(participantDTOs);

            // Assert
            Assert.AreEqual(relateCodes.Count, mappedParticipants.Count);
            for (var i = 0; i < relateCodes.Count; i++)
            {
                var participantUnderTest = participantDTOs.Single(p => p?.Name?.NameId == i);
                var mappedParticipant = mappedParticipants.Single(p => p.Participant.Person.Name.NameId == i);

                Assert.AreEqual(participantUnderTest.RelateCode.ToOwnerType(), mappedParticipant.OwnerType);
                VerifyIndividualParticipantsIdentical(mappedParticipant.Participant, participantUnderTest);
            }
        }

        [TestMethod]
        public void MapOwners_NoMatchingDTOs_ShouldReturnNull()
        {
            // Arrange
            var relateCodes = RelateCodes.OwnerRelateCodes;
            var participantDTOs = GenerateUnpopulatedParticipantWithRelateCode(relateCodes);

            // Act
            var mappedParticipants = Mapper.MapOwners(participantDTOs);

            // Assert
            Assert.IsNull(mappedParticipants);
        }

        [TestMethod]
        public void MapPayee_ShouldMap()
        {
            // Arrange
            var relateCodes = RelateCodes.PayeeRelateCodes;
            var participantDTOs = GetParticipantDTOsForParticipantMappingTests(relateCodes);

            // Act
            var mappedParticipant = Mapper.MapPayee(participantDTOs);

            // Assert
            var participantUnderTest = participantDTOs.Single(p => p?.Name?.NameId == 0);

            VerifyIndividualParticipantsIdentical(mappedParticipant.Participant, participantUnderTest);
        }

        [TestMethod]
        public void MapPayee_NoMatchingDTOs_ShouldReturnNull()
        {
            // Arrange
            var relateCodes = RelateCodes.PayeeRelateCodes;
            var participantDTOs = GenerateUnpopulatedParticipantWithRelateCode(relateCodes);

            // Act
            var mappedParticipants = Mapper.MapPayee(participantDTOs);

            // Assert
            Assert.IsNull(mappedParticipants);
        }

        [TestMethod]
        public void MapPayors_ShouldMap()
        {
            // Arrange
            var relateCodes = RelateCodes.PayorRelateCodes;
            var participantDTOs = GetParticipantDTOsForParticipantMappingTests(relateCodes);

            // Act
            var mappedParticipants = Mapper.MapPayors(participantDTOs);

            // Assert
            Assert.AreEqual(relateCodes.Count, mappedParticipants.Count);
            for (var i = 0; i < relateCodes.Count; i++)
            {
                var participantUnderTest = participantDTOs.Single(p => p?.Name?.NameId == i);
                var mappedParticipant = mappedParticipants.Single(p => p.Participant.Person.Name.NameId == i);

                Assert.AreEqual(participantUnderTest.RelateCode.ToPayorType(), mappedParticipant.PayorType);
                VerifyIndividualParticipantsIdentical(mappedParticipant.Participant, participantUnderTest);
            }
        }

        [TestMethod]
        public void MapPayors_NoMatchingDTOs_ShouldReturnNull()
        {
            // Arrange
            var relateCodes = RelateCodes.PayorRelateCodes;
            var participantDTOs = GenerateUnpopulatedParticipantWithRelateCode(relateCodes);

            // Act
            var mappedParticipants = Mapper.MapPayors(participantDTOs);

            // Assert
            Assert.IsNull(mappedParticipants);
        }

        [TestMethod]
        public void MapAgentFromFullAgentDTO_IsServicingAgent_IsWritingAgent_ShouldMap()
        {
            // Arrange
            var fullAgentDto = new PolicyAgentDTO
            {
                AgentNumber = "ABC",
                CommissionPercent = 0.5m,
                Level = "1",
                MarketCode = "NY",
                ServiceAgentIndicator = "X",
                Name = new NameDTO
                {
                    BusinessEmailAdress = "biz@biz.com",
                    NameFormatCode = "B",
                    NameId = 5,
                    NameBusiness = "Business Name"
                }
            };

            // Act
            var mappedAgent = Mapper.MapAgent(fullAgentDto);

            // Assert
            Assert.IsNotNull(mappedAgent);
            Assert.AreEqual(fullAgentDto.AgentNumber, mappedAgent.AgentId);
            Assert.AreEqual(fullAgentDto.MarketCode, mappedAgent.MarketCode);
            Assert.AreEqual(fullAgentDto.Level, mappedAgent.Level);
            Assert.IsFalse(mappedAgent.IsJustInTimeAgent);
            Assert.IsTrue(mappedAgent.IsServicingAgent);
            Assert.IsTrue(mappedAgent.IsWritingAgent);
            Assert.AreEqual(fullAgentDto.Name.NameBusiness, mappedAgent.Participant.Business.Name.BusinessName);
            Assert.AreEqual(fullAgentDto.Name.NameId, mappedAgent.Participant.Business.Name.NameId);
            Assert.AreEqual(fullAgentDto.Name.BusinessEmailAdress, mappedAgent.Participant.Business.EmailAddress);
        }

        [TestMethod]
        public void MapAgentFromFullAgentDTO_IsNotServicingAgent_IsNotWritingAgent_ShouldMap()
        {
            // Arrange
            var fullAgentDto = new PolicyAgentDTO
            {
                AgentNumber = "ABC",
                CommissionPercent = 0,
                Level = "1",
                MarketCode = "NY",
                ServiceAgentIndicator = "P",
                Name = new NameDTO
                {
                    BusinessEmailAdress = "biz@biz.com",
                    NameFormatCode = "B",
                    NameId = 5,
                    NameBusiness = "Business Name"
                }
            };

            // Act
            var mappedAgent = Mapper.MapAgent(fullAgentDto);

            // Assert
            Assert.IsNotNull(mappedAgent);
            Assert.AreEqual(fullAgentDto.AgentNumber, mappedAgent.AgentId);
            Assert.AreEqual(fullAgentDto.MarketCode, mappedAgent.MarketCode);
            Assert.AreEqual(fullAgentDto.Level, mappedAgent.Level);
            Assert.IsFalse(mappedAgent.IsServicingAgent);
            Assert.IsFalse(mappedAgent.IsWritingAgent);
            Assert.AreEqual(fullAgentDto.Name.NameBusiness, mappedAgent.Participant.Business.Name.BusinessName);
            Assert.AreEqual(fullAgentDto.Name.NameId, mappedAgent.Participant.Business.Name.NameId);
            Assert.AreEqual(fullAgentDto.Name.BusinessEmailAdress, mappedAgent.Participant.Business.EmailAddress);
        }

        [TestMethod]
        public void MapAgentFromJustInTimeAgentDTO_Individual_ShouldMap()
        {
            // Arrange
            var jitAgentDto = new JustInTimeAgentDTO
            {
                AgentId = "ABC",
                Level = "1",
                MarketCode = "NY",
            };

            var nameDto = new JustInTimeAgentNameDTO
            {
                FirstName = "John",
                LastName = "Taco",
                MiddleName = "Yum"
            };

            // Act
            var mappedAgent = Mapper.MapAgent(jitAgentDto, nameDto);

            // Assert
            Assert.IsNotNull(mappedAgent);
            Assert.AreEqual(jitAgentDto.AgentId, mappedAgent.AgentId);
            Assert.AreEqual(jitAgentDto.MarketCode, mappedAgent.MarketCode);
            Assert.AreEqual(jitAgentDto.Level, mappedAgent.Level);
            Assert.IsFalse(mappedAgent.IsServicingAgent);
            Assert.IsTrue(mappedAgent.IsJustInTimeAgent);
            Assert.IsTrue(mappedAgent.IsWritingAgent);

            Assert.AreEqual(nameDto.FirstName, mappedAgent.Participant.Person.Name.IndividualFirst);
            Assert.AreEqual(nameDto.MiddleName, mappedAgent.Participant.Person.Name.IndividualMiddle);
            Assert.AreEqual(nameDto.LastName, mappedAgent.Participant.Person.Name.IndividualLast);
        }

        [TestMethod]
        public void MapAgentFromJustInTimeAgentDTO_Business_ShouldMap()
        {
            // Arrange
            var jitAgentDto = new JustInTimeAgentDTO
            {
                AgentId = "ABC",
                Level = "1",
                MarketCode = "NY",
            };

            var nameDto = new JustInTimeAgentNameDTO
            {
                BusinessName = "Business Name"
            };

            // Act
            var mappedAgent = Mapper.MapAgent(jitAgentDto, nameDto);

            // Assert
            Assert.IsNotNull(mappedAgent);
            Assert.AreEqual(jitAgentDto.AgentId, mappedAgent.AgentId);
            Assert.AreEqual(jitAgentDto.MarketCode, mappedAgent.MarketCode);
            Assert.AreEqual(jitAgentDto.Level, mappedAgent.Level);
            Assert.IsFalse(mappedAgent.IsServicingAgent);
            Assert.IsTrue(mappedAgent.IsJustInTimeAgent);
            Assert.IsTrue(mappedAgent.IsWritingAgent);

            Assert.AreEqual(nameDto.BusinessName, mappedAgent.Participant.Business.Name.BusinessName);
        }

        [TestMethod]
        public void MapAgentFromJustInTimeAgentDTO_Pname_Business_ShouldMap()
        {
            // Arrange
            var jitAgentDto = new JustInTimeAgentDTO
            {
                AgentId = "ABC",
                Level = "1",
                MarketCode = "NY",
            };

            var pname = new PNAME
            {
                NAME_FORMAT_CODE = "B",
                NAME_BUSINESS = "Business Name",
                NAME_ID = 1
            };

            // Act
            var mappedAgent = Mapper.MapAgent(jitAgentDto, pname);

            // Assert
            Assert.IsNotNull(mappedAgent);
            Assert.AreEqual(jitAgentDto.AgentId, mappedAgent.AgentId);
            Assert.AreEqual(jitAgentDto.MarketCode, mappedAgent.MarketCode);
            Assert.AreEqual(jitAgentDto.Level, mappedAgent.Level);
            Assert.IsFalse(mappedAgent.IsServicingAgent);
            Assert.IsTrue(mappedAgent.IsJustInTimeAgent);
            Assert.IsTrue(mappedAgent.IsWritingAgent);

            Assert.IsTrue(mappedAgent.Participant.IsBusiness);
            Assert.AreEqual(pname.NAME_BUSINESS, mappedAgent.Participant.Business.Name.BusinessName);
            Assert.AreEqual(pname.NAME_ID, mappedAgent.Participant.Business.Name.NameId);
        }

        [TestMethod]
        public void MapAgentFromJustInTimeAgentDTO_Pname_Individual_ShouldMap()
        {
            // Arrange
            var jitAgentDto = new JustInTimeAgentDTO
            {
                AgentId = "ABC",
                Level = "1",
                MarketCode = "NY",
            };

            var pname = new PNAME
            {
                NAME_FORMAT_CODE = "I",
                INDIVIDUAL_FIRST = "First",
                INDIVIDUAL_MIDDLE = "M",
                INDIVIDUAL_LAST = "Last",
                INDIVIDUAL_PREFIX = "Pre",
                INDIVIDUAL_SUFFIX = "Suf"
            };

            // Act
            var mappedAgent = Mapper.MapAgent(jitAgentDto, pname);

            // Assert
            Assert.IsNotNull(mappedAgent);
            Assert.AreEqual(jitAgentDto.AgentId, mappedAgent.AgentId);
            Assert.AreEqual(jitAgentDto.MarketCode, mappedAgent.MarketCode);
            Assert.AreEqual(jitAgentDto.Level, mappedAgent.Level);
            Assert.IsFalse(mappedAgent.IsServicingAgent);
            Assert.IsTrue(mappedAgent.IsJustInTimeAgent);
            Assert.IsTrue(mappedAgent.IsWritingAgent);

            Assert.IsFalse(mappedAgent.Participant.IsBusiness);
            Assert.AreEqual(pname.INDIVIDUAL_FIRST, mappedAgent.Participant.Person.Name.IndividualFirst);
            Assert.AreEqual(pname.INDIVIDUAL_MIDDLE, mappedAgent.Participant.Person.Name.IndividualMiddle);
            Assert.AreEqual(pname.INDIVIDUAL_LAST, mappedAgent.Participant.Person.Name.IndividualLast);
            Assert.AreEqual(pname.INDIVIDUAL_SUFFIX, mappedAgent.Participant.Person.Name.IndividualSuffix);
            Assert.AreEqual(pname.INDIVIDUAL_PREFIX, mappedAgent.Participant.Person.Name.IndividualPrefix);
            Assert.AreEqual(pname.NAME_ID, mappedAgent.Participant.Person.Name.NameId);
        }

        private static ParticipantDTO GetIndividualParticipantDTO()
        {
            return new ParticipantDTO
            {
                BenefitSequenceNumber = 1,
                RelateCode = "IN",
                Addresses = new List<AddressDTO>
                {
                    new AddressDTO
                    {
                        AddressId = 1,
                        Line1 = "Line1",
                        Line2 = "Line2",
                        Line3 = "Line3",
                        City = "Omaha",
                        State = "NE",
                        Zip = "68102",
                        ZipExtension = "1234",
                        BoxNumber = "Box1",
                        Country = "USA",
                        TelephoneNumber = "4025551234"
                    }
                },
                Name = new NameDTO
                {
                    NameFormatCode = "I",
                    PersonalEmailAdress = "personalStuff@uwu.com",
                    IndividualFirst = "John",
                    IndividualLast = "Doe",
                    IndividualMiddle = "M",
                    IndividualSuffix = "Jr",
                    NameId = 1,
                },
                SexCode = "M",
                DateOfBirth = 19800101
            };
        }

        private static void VerifyIndividualParticipantsIdentical(Participant mappedParticipant, ParticipantDTO participantDto)
        {
            Assert.AreEqual(participantDto.Name.IndividualFirst, mappedParticipant.Person.Name.IndividualFirst);
            Assert.AreEqual(participantDto.Name.IndividualLast, mappedParticipant.Person.Name.IndividualLast);
            Assert.AreEqual(participantDto.Name.IndividualMiddle, mappedParticipant.Person.Name.IndividualMiddle);
            Assert.AreEqual(participantDto.Name.IndividualSuffix, mappedParticipant.Person.Name.IndividualSuffix);
            Assert.AreEqual(participantDto.Name.NameId, mappedParticipant.Person.Name.NameId);

            var expectedAddress = participantDto.Addresses.First();
            Assert.AreEqual(expectedAddress.AddressId, mappedParticipant.Address.AddressId);
            Assert.AreEqual(expectedAddress.Line1, mappedParticipant.Address.Line1);
            Assert.AreEqual(expectedAddress.Line2, mappedParticipant.Address.Line2);
            Assert.AreEqual(expectedAddress.Line3, mappedParticipant.Address.Line3);
            Assert.AreEqual(expectedAddress.City, mappedParticipant.Address.City);
            Assert.AreEqual(expectedAddress.State, mappedParticipant.Address.StateAbbreviation.ToString());
            Assert.AreEqual(expectedAddress.Zip, mappedParticipant.Address.ZipCode);
            Assert.AreEqual(expectedAddress.ZipExtension, mappedParticipant.Address.ZipExtension);
            Assert.AreEqual(expectedAddress.BoxNumber, mappedParticipant.Address.BoxNumber);
            Assert.AreEqual(expectedAddress.Country, mappedParticipant.Address.Country.ToString());

            var expectedPhoneNumber = expectedAddress.TelephoneNumber.ToPhoneNumber();
            Assert.AreEqual(expectedPhoneNumber, mappedParticipant.PhoneNumber);

            Assert.AreEqual(participantDto.SexCode.ToGender(), mappedParticipant.Person.Gender);
            Assert.AreEqual(participantDto.DateOfBirth.ToNullableDateTime(), mappedParticipant.Person.DateOfBirth);

            var expectedIsBusiness = participantDto.Name.NameFormatCode == "B";
            Assert.AreEqual(expectedIsBusiness, mappedParticipant.IsBusiness);
        }

        private static List<ParticipantDTO> GetParticipantDTOsForParticipantMappingTests(List<string> relateCodes)
        {
            var participantDtos = relateCodes.Select(GeneratePopulatedParticipantWithRelateCode).ToList();
            participantDtos.AddRange(GenerateUnpopulatedParticipantWithRelateCode(relateCodes));

            return participantDtos;
        }

        private static ParticipantDTO GeneratePopulatedParticipantWithRelateCode(string relateCodeUnderTest, int i)
        {
            var testParticipant = GetIndividualParticipantDTO();
            testParticipant.RelateCode = relateCodeUnderTest;
            testParticipant.Name.NameId = i;

            return testParticipant;
        }

        private static List<ParticipantDTO> GenerateUnpopulatedParticipantWithRelateCode(List<string> relateCodesToExclude)
        {
            var relateCodes = new List<string>();
            relateCodes.AddRange(RelateCodes.AnnuitantRelateCodes);
            relateCodes.AddRange(RelateCodes.AssigneeRelateCodes);
            relateCodes.AddRange(RelateCodes.BeneficiaryRelateCodes);
            relateCodes.AddRange(RelateCodes.OwnerRelateCodes);
            relateCodes.AddRange(RelateCodes.PayeeRelateCodes);
            relateCodes.AddRange(RelateCodes.PayorRelateCodes);
            relateCodes.AddRange(RelateCodes.InsuredRelateCodes);

            return relateCodes
                .Except(relateCodesToExclude)
                .Select(r => new ParticipantDTO { RelateCode = r })
                .ToList();
        }
    }
}
