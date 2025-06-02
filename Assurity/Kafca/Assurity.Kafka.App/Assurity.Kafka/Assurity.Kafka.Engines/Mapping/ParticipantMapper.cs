namespace Assurity.Kafka.Engines.Mapping
{
    using System.Collections.Generic;
    using System.Linq;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using NewRelic.Api.Agent;

    public class ParticipantMapper : IParticipantMapper
    {
        [Trace]
        public Participant MapParticipant(ParticipantDTO participantDto)
        {
            var pname = participantDto.Name;
            var participant = MapParticipantFromName(pname);

            participant.IsBusiness = pname.NameFormatCode == "B";

            if (!participant.IsBusiness)
            {
                participant.Person.Gender = participantDto.SexCode.ToGender();
                participant.Person.DateOfBirth = participantDto.DateOfBirth.ToNullableDateTime();
            }

            foreach (var address in participantDto.Addresses ?? new List<AddressDTO>())
            {
                participant.Address = new Address
                {
                    AddressId = address.AddressId,
                    Line1 = address.Line1?.Trim(),
                    Line2 = address.Line2?.Trim(),
                    Line3 = address.Line3?.Trim(),
                    City = address.City?.Trim(),
                    StateAbbreviation = address.State?.ToState(),
                    ZipCode = address.Zip?.Trim(),
                    ZipExtension = address.ZipExtension?.Trim(),
                    BoxNumber = address.BoxNumber?.Trim(),
                    Country = address.Country?.ToCountry()
                };

                participant.PhoneNumber = address.TelephoneNumber?.ToPhoneNumber();
            }

            return participant;
        }

        [Trace]
        public List<Insured> MapInsureds(
            List<ParticipantDTO> participantDtos,
            List<BenefitDTO> benefitDtos)
        {
            var filteredParticipants = FilterRelationshipDTOsByRelateCode(participantDtos, RelateCodes.InsuredRelateCodes);
            if (filteredParticipants == null || !filteredParticipants.Any())
            {
                return null;
            }

            var nameIdsProcessed = new List<int>();
            var insureds = new List<Insured>();
            foreach (var participant in filteredParticipants)
            {
                if (nameIdsProcessed.Contains(participant.Name.NameId))
                {
                    continue;
                }

                var pbenPolicyBenefit = benefitDtos?.FirstOrDefault(p => p.BenefitSequence == participant.BenefitSequenceNumber);
                var insured = new Insured();

                // Joint Equal Insureds are on benefit 0 in PRELA.
                if (participant.RelateCode == "JE" && pbenPolicyBenefit == null)
                {
                    insured.RelationshipToPrimaryInsured = RelationshipToPrimaryInsured.Joint;
                    insured.Participant = MapParticipant(participant);
                }
                else if (pbenPolicyBenefit == null)
                {
                    continue;
                }
                else if (participant.RelateCode == "IN")
                {
                    var relationshipToPrimaryInsured =
                        participant.BenefitSequenceNumber == 1
                            ? RelationshipToPrimaryInsured.Self
                            : RelationshipToPrimaryInsured.Additional;

                    insured.RelationshipToPrimaryInsured = relationshipToPrimaryInsured;
                    insured.Participant = MapParticipant(participant);
                }
                else if (participant.RelateCode == "ML")
                {
                    var matchingMultipleInsureds = FilterPmuinMultipleInsureds(
                        participant.Name.NameId,
                        pbenPolicyBenefit);

                    if (matchingMultipleInsureds.Count == 0)
                    {
                        continue;
                    }

                    foreach (var record in matchingMultipleInsureds)
                    {
                        if (nameIdsProcessed.Contains(record.NameId))
                        {
                            continue;
                        }

                        var multipleInsured = new Insured
                        {
                            RelationshipToPrimaryInsured = record.RelationshipToPrimaryInsured.Trim().ToRelationshipToPrimaryInsured(),
                            Participant = MapParticipant(participant)
                        };

                        nameIdsProcessed.Add(participant.Name.NameId);
                        if (ShouldAddInsured(multipleInsured))
                        {
                            insureds.Add(multipleInsured);
                        }
                    }
                }

                nameIdsProcessed.Add(participant.Name.NameId);
                if (ShouldAddInsured(insured))
                {
                    insureds.Add(insured);
                }
            }

            return
                insureds
                .OrderBy(i => i.RelationshipToPrimaryInsured)
                .ToList();
        }

        [Trace]
        public List<Annuitant> MapAnnuitants(List<ParticipantDTO> relationshipDTOs)
        {
            var annuitants = FilterRelationshipDTOsByRelateCode(relationshipDTOs, RelateCodes.AnnuitantRelateCodes);
            if (annuitants == null || !annuitants.Any())
            {
                return null;
            }

            return annuitants
                .Select(dto => new Annuitant
                {
                    AnnuitantType = dto.RelateCode.ToAnnuitantType(),
                    Participant = MapParticipant(dto)
                })
                .OrderBy(a => a.AnnuitantType)
                .ToList();
        }

        public Assignee MapAssignee(List<ParticipantDTO> relationshipDtos)
        {
            var assignees = FilterRelationshipDTOsByRelateCode(relationshipDtos, RelateCodes.AssigneeRelateCodes);
            if (assignees == null || !assignees.Any())
            {
                return null;
            }

            return new Assignee
            {
                Participant = MapParticipant(assignees[0])
            };
        }

        [Trace]
        public List<Beneficiary> MapBeneficiaries(List<ParticipantDTO> relationshipDtos)
        {
            var beneficiaries = FilterRelationshipDTOsByRelateCode(relationshipDtos, RelateCodes.BeneficiaryRelateCodes);
            if (beneficiaries == null || !beneficiaries.Any())
            {
                return null;
            }

            return beneficiaries
                .Select(prela =>
                {
                    return new Beneficiary
                    {
                        BeneficiaryType = prela.RelateCode.ToBeneficiaryType(),
                        Participant = MapParticipant(prela)
                    };
                })
                .OrderBy(b => b.BeneficiaryType)
                .ToList();
        }

        [Trace]
        public Payee MapPayee(List<ParticipantDTO> relationshipDtos)
        {
            var payees = FilterRelationshipDTOsByRelateCode(relationshipDtos, RelateCodes.PayeeRelateCodes);
            if (payees == null || !payees.Any())
            {
                return null;
            }

            return new Payee
            {
                Participant = MapParticipant(payees[0])
            };
        }

        [Trace]
        public List<Owner> MapOwners(List<ParticipantDTO> relationshipDtos)
        {
            var owners = FilterRelationshipDTOsByRelateCode(relationshipDtos, RelateCodes.OwnerRelateCodes);
            if (owners == null || !owners.Any())
            {
                return null;
            }

            return owners
                .Select(prela => new Owner
                {
                    OwnerType = prela.RelateCode.ToRelateCode(),
                    Participant = MapParticipant(prela)
                })
                .OrderBy(prela => prela.OwnerType)
                .ToList();
        }

        [Trace]
        public List<Payor> MapPayors(List<ParticipantDTO> relationshipDtos)
        {
            var payors = FilterRelationshipDTOsByRelateCode(relationshipDtos, RelateCodes.PayorRelateCodes);
            if (payors == null || !payors.Any())
            {
                return null;
            }

            return payors
                .Select(prela => new Payor
                {
                    PayorType = prela.RelateCode.ToPayorType(),
                    Participant = MapParticipant(prela)
                })
                .OrderBy(p => p.PayorType)
                .ToList();
        }

        [Trace]
        public Agent MapAgent(PolicyAgentDTO fullAgentDto)
        {
            var isServicingAgent = fullAgentDto.ServiceAgentIndicator.ToUpper() == "X";
            var isWritingAgent = fullAgentDto.CommissionPercent != 0;

            // TODO: "Writing Agent" means different things to different people and processes. We may want to change the name of this field once we have consensus. We probably also want to add CommissionPercentage and IsPreviousServicingAgent fields.
            // The LifePro system considers every agent on a commission record to be a writing agent. However, it only stores each agent once in the PRELA table, so an agent that is also a servicing agent is instead considered a servicing agent.
            // The Agent Contracting department considers agents that signed an application to be writing agents and other agents with commission records to be commission agents.
            // This application considers agents on commission records with a greater-than-zero commission percentage to be writing agents.
            return new Agent
            {
                AgentId = fullAgentDto.AgentNumber,
                MarketCode = fullAgentDto.MarketCode,
                Level = fullAgentDto.Level,
                IsServicingAgent = isServicingAgent,
                IsWritingAgent = isWritingAgent,
                Participant = MapParticipantFromName(fullAgentDto.Name)
            };
        }

        [Trace]
        public Agent MapAgent(JustInTimeAgentDTO jitAgentDto, JustInTimeAgentNameDTO jitNameDto)
        {
            if (jitNameDto == null)
            {
                return null;
            }

            var agent = MapAgentFromJitDto(jitAgentDto);
            agent.Participant = MapParticipantFromJitAgentNameDTO(jitNameDto);

            return agent;
        }

        [Trace]
        public Agent MapAgent(JustInTimeAgentDTO jitAgentDto, PNAME pname)
        {
            var agent = MapAgentFromJitDto(jitAgentDto);
            if (pname == null)
            {
                return agent;
            }

            agent.Participant = new Participant
            {
                IsBusiness = pname.NAME_FORMAT_CODE == "B"
            };

            if (agent.Participant.IsBusiness)
            {
                agent.Participant.Business = new Business
                {
                    Name = new Name
                    {
                        NameId = pname.NAME_ID,
                        BusinessName = pname.NAME_BUSINESS
                    }
                };
            }
            else
            {
                agent.Participant.Person = new Person
                {
                    Name = new Name()
                    {
                        NameId = pname.NAME_ID,
                        IndividualPrefix = pname.INDIVIDUAL_PREFIX,
                        IndividualFirst = pname.INDIVIDUAL_FIRST,
                        IndividualMiddle = pname.INDIVIDUAL_MIDDLE,
                        IndividualLast = pname.INDIVIDUAL_LAST,
                        IndividualSuffix = pname.INDIVIDUAL_SUFFIX,
                    }
                };
            }

            return agent;
        }

        private static List<ParticipantDTO> FilterRelationshipDTOsByRelateCode(
            List<ParticipantDTO> relationshipDtos,
            List<string> relateCodes)
        {
            if (relationshipDtos.Count == 0)
            {
                return null;
            }

            return relationshipDtos
                .Where(dto => relateCodes.Contains(dto.RelateCode))
                .ToList();
        }

        private static Participant MapParticipantFromName(NameDTO nameDto)
        {
            var participant = new Participant
            {
                IsBusiness = nameDto.NameFormatCode == "B"
            };

            if (participant.IsBusiness)
            {
                participant.Business = new Business
                {
                    Name = new Name
                    {
                        BusinessName = nameDto.NameBusiness.Trim(),
                        NameId = nameDto.NameId,
                    },
                    EmailAddress = nameDto.BusinessEmailAdress.Trim()
                };
            }
            else
            {
                participant.Person = new Person
                {
                    Name = new Name()
                    {
                        NameId = nameDto.NameId,
                        IndividualPrefix = nameDto.IndividualPrefix?.Trim(),
                        IndividualFirst = nameDto.IndividualFirst?.Trim(),
                        IndividualMiddle = nameDto.IndividualMiddle?.Trim(),
                        IndividualLast = nameDto.IndividualLast?.Trim(),
                        IndividualSuffix = nameDto.IndividualSuffix?.Trim(),
                    },
                    EmailAddress = nameDto.PersonalEmailAdress?.Trim()
                };
            }

            return participant;
        }

        [Trace]
        private static Participant MapParticipantFromJitAgentNameDTO(JustInTimeAgentNameDTO jitAgentDto)
        {
            var participant = new Participant
            {
                IsBusiness = string.IsNullOrEmpty(jitAgentDto.FirstName),
            };

            if (participant.IsBusiness)
            {
                participant.Business = new Business
                {
                    Name = new Name
                    {
                        BusinessName = jitAgentDto.BusinessName
                    }
                };
            }
            else
            {
                participant.Person = new Person
                {
                    Name = new Name
                    {
                        IndividualFirst = jitAgentDto.FirstName,
                        IndividualLast = jitAgentDto.LastName,
                        IndividualMiddle = jitAgentDto.MiddleName,
                    }
                };
            }

            return participant;
        }

        private static Agent MapAgentFromJitDto(JustInTimeAgentDTO jitAgentDto)
        {
            return new Agent
            {
                AgentId = jitAgentDto.AgentId,
                MarketCode = jitAgentDto.MarketCode,
                Level = jitAgentDto.Level,
                IsServicingAgent = false,
                IsWritingAgent = true,
                IsJustInTimeAgent = true
            };
        }

        private static bool ShouldAddInsured(Insured insured)
        {
            return
                insured.Participant != null
                && (insured.Participant.Business != null || insured.Participant.Person != null);
        }

        /// <summary>
        /// Returns only Multiple Insureds which:
        /// - Match the current participant's nameID.
        /// - Have valid benefit data.
        /// </summary>
        /// <param name="participantNameId"></param>
        /// <param name="benefit"></param>
        /// <returns></returns>
        private static List<MultipleInsuredDTO> FilterPmuinMultipleInsureds(
            int participantNameId,
            BenefitDTO benefit)
        {
            if (benefit.MultipleInsureds == null || benefit.MultipleInsureds.Count == 0)
            {
                return new List<MultipleInsuredDTO>();
            }

            var matchingPmuins =
                benefit
                .MultipleInsureds
                .Where(pmuin => pmuin.NameId == participantNameId);

            if (benefit.StatusCode == "A" || benefit.StatusCode == "P")
            {
                var lifeProNow = DateTime.Now.ToLifeProDate();
                matchingPmuins = matchingPmuins.Where(insured => insured.StopDate > lifeProNow);
            }
            else if (benefit.StatusCode == "T")
            {
                matchingPmuins = matchingPmuins.Where(insured =>
                    benefit.StatusDate > insured.StartDate
                    || benefit.StatusDate < insured.StopDate);
            }

            return matchingPmuins.ToList();
        }
    }
}
