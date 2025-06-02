using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Assurity.Kafka.Engines.Tests")]

namespace Assurity.Kafka.Engines.Policy
{
    using System;
    using System.Collections.Generic;
    using Assurity.Kafka.Accessors;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Engines.Mapping;
    using Assurity.Kafka.Utilities.Config;
    using Assurity.Kafka.Utilities.Constants;
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;
    using Microsoft.Extensions.Logging;
    using NewRelic.Api.Agent;

    public class ConsumerPolicyEngine : BasePolicyEngine, IConsumerPolicyEngine
    {
        private const bool IsMigrationWorker = false;

        public ConsumerPolicyEngine(
            ILifeProAccessor lifeProAccessor,
            IGlobalDataAccessor globalDataAccessor,
            ISupportDataAccessor supportDataAccessor,
            IEventsAccessor eventsAccessor,
            IConfigurationManager configurationManager,
            ILogger<BasePolicyEngine> logger,
            IPolicyMapper policyMapper,
            ILogger<ConsumerPolicyEngine> consumerLogger)
            : base(null, lifeProAccessor, globalDataAccessor, supportDataAccessor, eventsAccessor, configurationManager, logger, policyMapper)
        {
            LifeProAccessor = lifeProAccessor;
            PolicyMapper = policyMapper;
            Logger = consumerLogger;
        }

        private ILogger<ConsumerPolicyEngine> Logger { get; set; }

        private ILifeProAccessor LifeProAccessor { get; set; }

        private IPolicyMapper PolicyMapper { get; set; }

        [Trace]
        public async Task<List<Agent>> GetAgents(string policyNumber, string companyCode, DateTime applicationDate)
        {
            return await GetAgents(IsMigrationWorker, policyNumber, companyCode, applicationDate);
        }

        [Trace]
        public List<Annuitant> GetAnnuitants(string policyNumber, string companyCode)
        {
            var participants = LifeProAccessor.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.AnnuitantRelateCodes);

            return PolicyMapper.MapAnnuitants(participants);
        }

        [Trace]
        public Assignee GetAssignee(string policyNumber, string companyCode)
        {
            var participants = LifeProAccessor.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.AssigneeRelateCodes);

            return PolicyMapper.MapAssignee(participants);
        }

        [Trace]
        public List<Beneficiary> GetBeneficiaries(string policyNumber, string companyCode)
        {
            var participants = LifeProAccessor.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.BeneficiaryRelateCodes);

            return PolicyMapper.MapBeneficiaries(participants);
        }

        [Trace]
        public Payee GetPayee(string policyNumber, string companyCode)
        {
            var participants = LifeProAccessor.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.PayeeRelateCodes);

            return PolicyMapper.MapPayee(participants);
        }

        [Trace]
        public List<Owner> GetOwners(string policyNumber, string companyCode)
        {
            var participants = LifeProAccessor.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.OwnerRelateCodes);

            return PolicyMapper.MapOwners(participants);
        }

        [Trace]
        public List<Payor> GetPayors(string policyNumber, string companyCode)
        {
            var participants = LifeProAccessor.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.PayorRelateCodes);

            return PolicyMapper.MapPayors(participants);
        }

        [Trace]
        public List<Insured> GetInsureds(string policyNumber, string companyCode)
        {
            var participants = LifeProAccessor.GetParticipantDTOs(policyNumber, companyCode, RelateCodes.InsuredRelateCodes);
            var benefits = LifeProAccessor.GetBenefitDTOs(policyNumber, companyCode);

            return PolicyMapper.MapInsureds(participants, benefits);
        }

        [Trace]
        public async Task<(GetPolicyResult, Policy?)> GetPolicy(string policyNumber, string companyCode)
        {
            return await GetPolicy(false, policyNumber, companyCode);
        }

        [Trace]
        public async Task<PolicyStatusDetail> GetPolicyStatusDetail(string policyNumber, string companyCode)
        {
            return await GetPolicyStatusDetail(IsMigrationWorker, policyNumber, companyCode);
        }

        [Trace]
        public async Task<Employer> GetEmployer(string policyNumber, string companyCode)
        {
            var employerDetail = await LifeProAccessor.GetEmployerDetail(policyNumber, companyCode);
            if (employerDetail == null)
            {
                return null;
            }

            return PolicyMapper.MapEmployer(employerDetail);
        }

        [Trace]
        public Benefit GetBenefit(
            CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber,
            LineOfBusiness lineOfBusiness,
            PPBEN_POLICY_BENEFITS ppben)
        {
            var benefitDto = LifeProAccessor.GetBenefitDTO(
                companyCodeAndPolicyNumber.PolicyNumber,
                companyCodeAndPolicyNumber.CompanyCode,
                ppben.PBEN_ID);

            if (benefitDto == null)
            {
                Logger.LogWarning(
                    "Unable to find benefit with PBEN_ID: {PBEN_ID} for PolicyNumber {policyNumber} and CompanyCode {companyCode}.",
                    ppben.PBEN_ID,
                    companyCodeAndPolicyNumber.PolicyNumber,
                    companyCodeAndPolicyNumber.CompanyCode);

                return null;
            }

            var extendedKeysLookup =
                GenerateExtendedKeysLookup(benefitDto.MultipleInsureds ?? new List<MultipleInsuredDTO>());

            var lookupData = LifeProAccessor.GetExtendedKeyData(extendedKeysLookup);

            return PolicyMapper.MapBenefit(lineOfBusiness, benefitDto, lookupData);
        }

        [Trace]
        public async Task<List<Requirement>> GetRequirements(Policy policy)
        {
            var participantDtos = LifeProAccessor.GetParticipantDTOs(policy.PolicyNumber, policy.CompanyCode);

            return await GetRequirements(false, policy, participantDtos);
        }

        [Trace]
        public (ReturnPaymentType returnPaymentType, DateTime? returnPaymentDate) GetReturnPaymentData(string policyNumber, string companyCode)
        {
            return GetReturnPaymentData(IsMigrationWorker, policyNumber, companyCode);
        }

        [Trace]
        public async Task DeletePolicy(string policyNumber, string companyCode)
        {
            await DeleteAllPolicyData(policyNumber, companyCode);
        }

        [Trace]
        public async Task<ProductDescription> GetBaseProductDescriptionByPlanCode(string planCode)
        {
            return await GetBaseProductDescriptionByPlanCode(IsMigrationWorker, planCode);
        }

        [Trace]
        public List<Policy> UpdateRequirementName(List<Policy> policies, short reqNumber, string reqDescription)
        {
            return UpdateRequirementNames(policies, reqNumber, reqDescription);
        }
    }
}