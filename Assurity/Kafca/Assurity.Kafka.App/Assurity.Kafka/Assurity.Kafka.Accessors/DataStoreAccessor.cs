namespace Assurity.Kafka.Accessors
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors.Context;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Utilities;
    using Assurity.Kafka.Utilities.Extensions;
    using Assurity.PolicyInfo.Contracts.V1;
    using Microsoft.EntityFrameworkCore;
    using NewRelic.Api.Agent;

    /// <summary>
    /// Accesses replicated LifePro data. Not "real time" but practically very close.
    /// </summary>
    public class DataStoreAccessor : BaseAccessor, IDataStoreAccessor
    {
        public DataStoreAccessor(
            IDbContextFactory<DataStoreContext> dataStoreContextFactory)
        {
            DataStoreContextFactory = dataStoreContextFactory;
        }

        private IDbContextFactory<DataStoreContext> DataStoreContextFactory { get; }

        /// <summary>
        /// Gets the data needed for a policy object found in the PPOLC table.
        /// </summary>
        /// <param name="policyNumber">The policyNumber for the desired policy.</param>
        /// <param name="companyCode">The company code for the desired policy.</param>
        /// <returns>A custom DTO, PPOLCEntityQuery, build to hold the results of this query.</returns>
        [Trace]
        public async Task<PPOLC> GetPPOLC(string policyNumber, string companyCode)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetEligiblePPOLC(context, policyNumber, companyCode);
            }
        }

        [Trace]
        public async Task<ProductDescription> GetBaseProductDescriptionByProductCode(string productCode)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetBaseProductDescriptionByProductCode(context, productCode);
            }
        }

        [Trace]
        public async Task<ProductDescription> GetBaseProductDescriptionByPlanCode(string planCode)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetBaseProductDescriptionByPlanCode(context, planCode);
            }
        }

        [Trace]
        public async Task<PPEND_NEW_BUSINESS_PENDING> GetPPEND_NEW_BUSINESS_PENDING(string policyNumber)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPPEND_NEW_BUSINESS_PENDING(context, policyNumber);
            }
        }

        [Trace]
        public async Task<List<PRELA_RELATIONSHIP_MASTER>> GetPRELA_RELATIONSHIP_MASTER(string identifyingAlpha, List<string> relateCodeList)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPRELA_RELATIONSHIP_MASTER(context, identifyingAlpha, relateCodeList);
            }
        }

        [Trace]
        public List<ParticipantDTO> GetParticipantDTOs(
            string policyNumber,
            string companyCode,
            List<string> relateCodes)
        {
            using var context = DataStoreContextFactory.CreateDbContext();

            return GetParticipantDTOs(context, policyNumber, companyCode, relateCodes);
        }

        [Trace]
        public List<ParticipantDTO> GetParticipantDTOsWithoutAddress(
            string policyNumber,
            string companyCode,
            List<string> relateCodes)
        {
            using var context = DataStoreContextFactory.CreateDbContext();

            return GetParticipantDTOsWithoutAddress(context, policyNumber, companyCode, relateCodes);
        }

        [Trace]
        public async Task<List<PAGNT_AGENT_MASTER>> GetPAGNT_AGENT_MASTER(string companyCode, string agentNumber)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPAGNT_AGENT_MASTER(context, companyCode, agentNumber);
            }
        }

        [Trace]
        public async Task<List<PRELA_RELATIONSHIP_MASTER>> GetPRELA_RELATIONSHIP_MASTER_FOR_INSUREDS_ON_BENEFITS(string identifyingAlpha)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPRELA_RELATIONSHIP_MASTER_FOR_INSUREDS_ON_BENEFITS(context, identifyingAlpha);
            }
        }

        [Trace]
        public async Task<PRELA_RELATIONSHIP_MASTER?> GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
            string policyNumber,
            int nameId,
            string relateCode,
            short benefitSeqNumber)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPRELA_RELATIONSHIP_MASTER_BENEFIT(context, policyNumber, nameId, relateCode, benefitSeqNumber);
            }
        }

        [Trace]
        public async Task<PNAME> GetPNAME(int nameId)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPNAME(context, nameId);
            }
        }

        [Trace]
        public async Task<List<PNALK>> GetPNALK(int nameId)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPNALK(context, nameId);
            }
        }

        [Trace]
        public async Task<PADDR> GetPADDR(int addressId)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPADDR(context, addressId);
            }
        }

        [Trace]
        public List<PolicyAgentDTO> GetPolicyAgentDTOs(
            string policyNumber,
            string companyCode)
        {
            using var context = DataStoreContextFactory.CreateDbContext();

            return GetPolicyAgentDTOs(context, companyCode, policyNumber);
        }

        [Trace]
        public List<BenefitDTO> GetBenefitDTOs(string policyNumber, string companyCode)
        {
            using var context = DataStoreContextFactory.CreateDbContext();

            return GetBenefitDTOs(context, policyNumber, companyCode);
        }

        [Trace]
        public async Task<PPBEN_POLICY_BENEFITS?> GetPPBEN_POLICY_BENEFITS(
            string policyNumber,
            short benefitSeq,
            long benefitId,
            string companyCode)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPPBEN_POLICY_BENEFITS(context, policyNumber, benefitSeq, benefitId, companyCode);
            }
        }

        [Trace]
        public async Task<List<PPBEN_POLICY_BENEFITS>> GetPPBEN_POLICY_BENEFITS(string policyNumber, string companyCode)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPPBEN_POLICY_BENEFITS(context, policyNumber, companyCode);
            }
        }

        [Trace]
        public async Task<PPBEN_POLICY_BENEFITS_TYPES_BA_OR?> GetPPBEN_POLICY_BENEFIT_TYPES_BA_OR(
            long pbenID)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPPBEN_POLICY_BENEFIT_TYPES_BA_OR(context, pbenID);
            }
        }

        [Trace]
        public async Task<PPBEN_POLICY_BENEFITS_TYPES_BF?> GetPPBEN_POLICY_BENEFIT_TYPES_BF(
            long pbenID)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPPBEN_POLICY_BENEFIT_TYPES_BF(context, pbenID);
            }
        }

        [Trace]
        public async Task<PPBEN_POLICY_BENEFITS_TYPES_SU?> GetPPBEN_POLICY_BENEFIT_TYPES_SU(
           long pbenID)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPPBEN_POLICY_BENEFIT_TYPES_SU(context, pbenID);
            }
        }

        [Trace]
        public async Task<PPBEN_POLICY_BENEFITS_TYPES_SL?> GetPPBEN_POLICY_BENEFIT_TYPES_SL(
           long pbenID)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPPBEN_POLICY_BENEFIT_TYPES_SL(context, pbenID);
            }
        }

        [Trace]
        public async Task<PPBEN_POLICY_BENEFITS_TYPES_SP?> GetPPBEN_POLICY_BENEFIT_TYPES_SP(
           long pbenID)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPPBEN_POLICY_BENEFIT_TYPES_SP(context, pbenID);
            }
        }

        [Trace]
        public async Task<List<PMUIN_MULTIPLE_INSUREDS>> GetPMUIN_MULTIPLE_INSUREDS(
            string policyNumber,
            int benefitSeq,
            int nameId,
            string relateCode,
            string companyCode)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPMUIN_MULTIPLE_INSUREDS(context, policyNumber, benefitSeq, nameId, relateCode, companyCode);
            }
        }

        [Trace]
        public async Task<ExtendedKeyQueryEntity?> GetExtendedKey(string keyIdent, short iter, short ky)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();

            return await GetExtendedKey(context, keyIdent, iter, ky);
        }

        [Trace]
        public List<ExtendedKeyLookupResult> GetExtendedKeyData(ExtendedKeysLookup lookups)
        {
            using var context = DataStoreContextFactory.CreateDbContext();

            return GetExtendedKeyData(context, lookups);
        }

        [Trace]
        public async Task<string?> GetUnderwritingClassDescription(string planCode, string underwritingClass)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetUnderwritingClassDescription(context, planCode, underwritingClass);
            }
        }

        [Trace]
        public async Task<List<AgentDTO>> GetAgents(
            string policyNumber,
            string companyCode)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetAgents(context, companyCode, policyNumber);
            }
        }

        [Trace]
        public HashSet<CompanyCodeAndPolicyNumber> GetMigratablePPOLCRecords()
        {
            var context = DataStoreContextFactory.CreateDbContext();
            var intPastDate = DateTimeUtility.DateTimeNowCentral().AddYears(-3).ToLifeProDate();
            const string terminated = "T";
            const string error = "ER";

            // Only collect policies that are not terminated, or were terminated recently and not due to error.
            return context.PPOLC
                .Where(policy =>
                    !policy.POLICY_NUMBER.StartsWith("G")
                    && policy.CONTRACT_REASON != error
                    && !(policy.CONTRACT_CODE == terminated
                        && policy.CONTRACT_DATE <= intPastDate
                        && string.IsNullOrWhiteSpace(policy.GROUP_NUMBER))
                    && policy.APPLICATION_DATE.ToString().Length == 8)
                .Select(policy => new CompanyCodeAndPolicyNumber
                {
                    CompanyCode = policy.COMPANY_CODE.Trim(),
                    PolicyNumber = policy.POLICY_NUMBER.Trim()
                })
                .ToHashSet();
        }

        [Trace]
        public async Task<Agent> GetAgentUpline(string agentNum, string marketCode, string agentLevel, DateTime applicationDate)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetAgentUpline(context, agentNum, marketCode, agentLevel, applicationDate);
            }
        }

        [Trace]
        public PolicyDTO GetPolicyDTO(string policyNumber, string companyCode)
        {
            using var context = DataStoreContextFactory.CreateDbContext();

            return GetPolicyDTO(context, policyNumber, companyCode);
        }

        [Trace]
        public async Task<List<PolicyRelationship>> GetPolicyRelationships(
            int nameId,
            int addressId)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPolicyRelationships(context, nameId, addressId);
            }
        }

        [Trace]
        public async Task<string?> GetPolicyStatusDetail(string policyNumber, string companyCode)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPolicyStatusDetail(context, policyNumber, companyCode);
            }
        }

        [Trace]
        public async Task<EmployerDTO?> GetEmployerDetail(string policyNumber, string companyCode)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetEmployerDetail(context, policyNumber, companyCode);
            }
        }

        [Trace]
        public async Task<List<PolicyRequirement>> GetPolicyRequirementsForHealth(
            CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPolicyRequirementsForHealth(context, companyCodeAndPolicyNumber);
            }
        }

        [Trace]
        public async Task<List<PolicyRequirement>> GetPolicyRequirementsForLife(
            CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetPolicyRequirementsForLife(context, companyCodeAndPolicyNumber);
            }
        }

        [Trace]
        public async Task<string?> GetCoverageDescription(string planCode)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetCoverageDescription(context, planCode);
            }
        }

        [Trace]
        public List<PaymentDTO> GetPaymentData(string policyNumber, string companyCode)
        {
            using var context = DataStoreContextFactory.CreateDbContext();

            return GetPaymentData(context, policyNumber, companyCode);
        }

        [Trace]
        public bool IsInitialPaymentDeclined(string policyNumber, string companyCode)
        {
            using var context = DataStoreContextFactory.CreateDbContext();

            return IsInitialPaymentDeclined(context, policyNumber, companyCode);
        }

        [Trace]
        public async Task<bool> IsULBenefitInGracePeriod(long benefitId)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await IsULBenefitInGracePeriod(context, benefitId);
            }
        }

        [Trace]
        public async Task<PACON_ANNUITY_POLICY?> GetAnnuityPolicy(string policyNumber, string companyCode)
        {
            using var context = await DataStoreContextFactory.CreateDbContextAsync();
            {
                return await GetAnnuityPolicy(context, policyNumber, companyCode);
            }
        }

        [Trace]
        public async Task AddSystemDataLoadWithAgentHierarchyChange(
            PHIER_AGENT_HIERARCHY phier,
            ChangeType changeType,
            string beforeAgentId)
        {
            using var dataStoreContext = await DataStoreContextFactory.CreateDbContextAsync();
            {
                var systemDataLoad = new SystemDataLoad
                {
                    AgentHierarchyChange = new AgentHierarchyChange
                    {
                        CompanyCode = phier.COMPANY_CODE,
                        AgentId = phier.AGENT_NUM.Trim(),
                        MarketCode = phier.MARKET_CODE.Trim(),
                        AgentLevel = phier.AGENT_LEVEL,
                        StartDate = phier.START_DATE,
                        StopDate = phier.STOP_DATE,
                        UplineAgentId = phier.HIERARCHY_AGENT.Trim(),
                        UplineMarketCode = phier.HIER_MARKET_CODE.Trim(),
                        UplineAgentLevel = phier.HIER_AGENT_LEVEL.Trim(),
                        BeforeAgentId = beforeAgentId?.Trim(),
                        ChangeType = changeType
                    },

                    // Just to be safe, allow a bit of time for replication, since the Kafka Consumer
                    // has CDC on lpsqldev.LPDEV while the Commissions/Debt hierarchy uses DMDev.DataStore
                    InitiatedDate = DateTimeUtility.DateTimeNowCentral().AddMinutes(5),
                    LoadType = LoadType.Change,
                    Status = SystemDataLoadStatus.Initiated
                };

                await dataStoreContext.SystemDataLoads.AddAsync(systemDataLoad);

                await dataStoreContext.SaveChangesAsync();
            }
        }
    }
}