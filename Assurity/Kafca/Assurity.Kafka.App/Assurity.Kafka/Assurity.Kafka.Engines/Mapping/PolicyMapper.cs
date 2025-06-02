namespace Assurity.Kafka.Engines.Mapping
{
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.DataTransferObjects.Requirements;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using NewRelic.Api.Agent;

    public class PolicyMapper : IPolicyMapper
    {
        public PolicyMapper(
            IBenefitMapper benefitMapper,
            IParticipantMapper participantMapper,
            IRequirementsMapper requirementMapper)
        {
            BenefitMapper = benefitMapper;
            ParticipantMapper = participantMapper;
            RequirementsMapper = requirementMapper;
        }

        private IBenefitMapper BenefitMapper { get; }

        private IParticipantMapper ParticipantMapper { get; }

        private IRequirementsMapper RequirementsMapper { get; }

        [Trace]
        public Policy MapPolicy(PolicyDTO policyDto)
        {
            var policy = new Policy
            {
                AnnualPremium = policyDto.AnnualPremium,
                ApplicationDate = policyDto.ApplicationDate.ToNullableDateTime(),
                ApplicationReceivedDate = policyDto.AppReceivedDate.ToNullableDateTime(),
                BillingDay = MapBillingDay(policyDto),
                BillingForm = policyDto.BillingForm.ToBillingForm(),
                BillingMode = policyDto.BillingMode.ToBillingMode(policyDto.PolcSpecialMode),
                BillingStatus = policyDto.BillingCode.ToBillingCode(),
                BillingReason = policyDto.BillingReason.ToBillingReason(),
                CompanyCode = policyDto.CompanyCode,
                Employer = MapEmployer(policyDto.Employer),
                IssueState = policyDto.IssueState.ToState(),
                LineOfBusiness = policyDto.LineOfBusiness.ToLineOfBusiness(),
                ModePremium = policyDto.ModePremium,
                PaidToDate = MapPaidToDate(policyDto),
                PolicyNumber = policyDto.PolicyNumber,
                ProductCode = policyDto.ProductCode,
                ResidentState = policyDto.ResidenceState.ToState()
            };

            if (policy.LineOfBusiness == LineOfBusiness.ImmediateAnnuity && policyDto.AnnuityPolicy != null)
            {
                policy.IssueDate = policyDto.AnnuityPolicy.IssueDate.ToNullableDateTime();
                policy.PolicyStatus = policyDto.AnnuityPolicy.StatusCode.ToPolicyStatus();
                policy.PolicyStatusReason = policyDto.AnnuityPolicy.StatusReason.ToPolicyStatusReason(policyDto.AnnuityPolicy.StatusCode);
                policy.TaxQualificationStatus = policyDto.AnnuityPolicy.TaxQualification.ToTaxQualificationStatus(policyDto.LineOfBusiness);
                policy.TerminationDate = GetTerminationDate(policyDto.AnnuityPolicy.StatusCode, policyDto.AnnuityPolicy.StatusDate);
            }
            else
            {
                policy.IssueDate = policyDto.IssueDate.ToNullableDateTime();
                policy.PolicyStatus = policyDto.ContractCode.ToPolicyStatus();
                policy.PolicyStatusReason = policyDto.ContractReason.ToPolicyStatusReason(policyDto.ContractCode);
                policy.TaxQualificationStatus = policyDto.TaxQualifyCode.ToTaxQualificationStatus(policyDto.LineOfBusiness);
                policy.TerminationDate = GetTerminationDate(policyDto.ContractCode, policyDto.ContractDate);
            }

            if (policyDto.NewBusinessPending != null)
            {
                policy.SubmitDate = policyDto.NewBusinessPending.RequirementDate.ToNullableDateTime();
            }

            return policy;
        }

        [Trace]
        public Employer MapEmployer(EmployerDTO employerDto)
        {
            if (employerDto == null)
            {
                return null;
            }

            return new Employer
            {
                Number = employerDto.GroupNumber?.Trim(),
                Business = new Business
                {
                    EmailAddress = employerDto.BusinessEmailAddress?.Trim(),
                    Name = new Name
                    {
                        BusinessName = employerDto.EmployerName?.Trim(),
                        NameId = employerDto.NameId
                    }
                },
                Status = MapEmployerStatus(employerDto.StatusCode)
            };
        }

        [Trace]
        public Benefit MapBenefit(LineOfBusiness lineOfBusiness, BenefitDTO benefitDto, List<ExtendedKeyLookupResult> extendedKeyData)
        {
            return BenefitMapper.MapBenefit(lineOfBusiness, benefitDto, extendedKeyData);
        }

        [Trace]
        public List<Annuitant> MapAnnuitants(List<ParticipantDTO> relationshipDTOs)
        {
            return ParticipantMapper.MapAnnuitants(relationshipDTOs);
        }

        [Trace]
        public Assignee MapAssignee(List<ParticipantDTO> relationshipDtos)
        {
            return ParticipantMapper.MapAssignee(relationshipDtos);
        }

        [Trace]
        public List<Beneficiary> MapBeneficiaries(List<ParticipantDTO> relationshipDtos)
        {
            return ParticipantMapper.MapBeneficiaries(relationshipDtos);
        }

        [Trace]
        public List<Insured> MapInsureds(List<ParticipantDTO> participantDtos, List<BenefitDTO> benefitDtos)
        {
            return ParticipantMapper.MapInsureds(participantDtos, benefitDtos);
        }

        [Trace]
        public List<Owner> MapOwners(List<ParticipantDTO> relationshipDtos)
        {
            return ParticipantMapper.MapOwners(relationshipDtos);
        }

        [Trace]
        public Payee MapPayee(List<ParticipantDTO> relationshipDtos)
        {
            return ParticipantMapper.MapPayee(relationshipDtos);
        }

        [Trace]
        public List<Payor> MapPayors(List<ParticipantDTO> relationshipDtos)
        {
            return ParticipantMapper.MapPayors(relationshipDtos);
        }

        [Trace]
        public Agent MapAgent(PolicyAgentDTO policyAgentDto)
        {
            return ParticipantMapper.MapAgent(policyAgentDto);
        }

        [Trace]
        public Agent MapAgent(JustInTimeAgentDTO jitAgentDto, JustInTimeAgentNameDTO jitNameDto)
        {
            return ParticipantMapper.MapAgent(jitAgentDto, jitNameDto);
        }

        [Trace]
        public Agent MapAgent(JustInTimeAgentDTO jitAgentDto, PNAME pname)
        {
            return ParticipantMapper.MapAgent(jitAgentDto, pname);
        }

        [Trace]
        public List<Requirement> MapRequirements(
            List<PolicyRequirement> policyRequirements,
            List<RequirementMapping> requirementMappings,
            List<ParticipantDTO> participants,
            List<GlobalRequirementLookupResult> globalCommentData)
        {
            return RequirementsMapper.MapRequirements(
                policyRequirements,
                requirementMappings,
                participants,
                globalCommentData);
        }

        [Trace]
        public Requirement GenerateHomeOfficeReviewRequirement()
        {
            return RequirementsMapper.GenerateHomeOfficeReviewRequirement();
        }

        private static EmployerStatus? MapEmployerStatus(char statusCode)
        {
            return statusCode switch
            {
                'A' => EmployerStatus.Active,
                'S' => EmployerStatus.Suspended,
                'T' => EmployerStatus.Terminated,
                _ => null
            };
        }

        private static DateTime? GetTerminationDate(string ppolcContractCode, int ppolcContractDate)
        {
            if (ppolcContractCode != null && ppolcContractCode.Equals("T", StringComparison.InvariantCultureIgnoreCase))
            {
                return ppolcContractDate.ToNullableDateTime();
            }

            return null;
        }

        private static short MapBillingDay(PolicyDTO policyDto)
        {
            if (policyDto.PolicyBillDay == 0 && policyDto.IssueDate.ToNullableDateTime() != null)
            {
                return (short)policyDto.IssueDate.ToNullableDateTime()?.Day;
            }

            return policyDto.PolicyBillDay;
        }

        private static DateTime? MapPaidToDate(PolicyDTO policyDto)
        {
            var lineOfBusiness = policyDto.LineOfBusiness.ToLineOfBusiness();

            return policyDto.PaidToDate.ToNullablePaidToDate(lineOfBusiness);
        }
    }
}
