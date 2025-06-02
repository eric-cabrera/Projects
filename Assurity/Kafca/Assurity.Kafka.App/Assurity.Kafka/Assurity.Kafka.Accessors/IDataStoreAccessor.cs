namespace Assurity.Kafka.Accessors
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.DataTransferObjects.Benefits;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.PolicyInfo.Contracts.V1;

    public interface IDataStoreAccessor
    {
        /// <summary>
        /// Adds a record to DataStore.commdebt.AgentHierarchyChange to capture the CDC
        /// of PHIER_AGENT_HIERARCHY and a record to DataStore.commdebt.SystemDataLoad to trigger
        /// the Assurity.Commissions.Debt.Identifier service to pick up the change and update the
        /// DataStore.commdebt.Agents and DataStore.commdebt.DebtAgents tables in order to keep the
        /// Debt security up-to-date.
        /// </summary>
        /// <param name="phier"></param>
        /// <param name="changeType"></param>
        /// <param name="beforeAgentId"></param>
        /// <returns></returns>
        Task AddSystemDataLoadWithAgentHierarchyChange(
            PHIER_AGENT_HIERARCHY phier,
            ChangeType changeType,
            string beforeAgentId);

        Task<PPOLC> GetPPOLC(string policyNumber, string companyCode);

        Task<ProductDescription> GetBaseProductDescriptionByProductCode(string productCode);

        Task<ProductDescription> GetBaseProductDescriptionByPlanCode(string planCode);

        Task<PPEND_NEW_BUSINESS_PENDING> GetPPEND_NEW_BUSINESS_PENDING(string policyNumber);

        Task<List<PRELA_RELATIONSHIP_MASTER>> GetPRELA_RELATIONSHIP_MASTER(string identifyingAlpha, List<string> relateCodeList);

        Task<List<PAGNT_AGENT_MASTER>> GetPAGNT_AGENT_MASTER(string companyCode, string agentNumber);

        Task<List<PRELA_RELATIONSHIP_MASTER>> GetPRELA_RELATIONSHIP_MASTER_FOR_INSUREDS_ON_BENEFITS(string identifyingAlpha);

        /// <summary>
        /// Gets a single PRELA_RELATIONSHIP_MASTER record with the requirement
        /// that it must have related benefit data.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="nameId"></param>
        /// <param name="relateCode"></param>
        /// <param name="benefitSeqNumber"></param>
        /// <returns></returns>
        Task<PRELA_RELATIONSHIP_MASTER?> GetPRELA_RELATIONSHIP_MASTER_BENEFIT(
            string policyNumber,
            int nameId,
            string relateCode,
            short benefitSeqNumber);

        Task<PNAME> GetPNAME(int nameId);

        Task<List<PNALK>> GetPNALK(int nameId);

        Task<PADDR> GetPADDR(int addressId);

        Task<List<PPBEN_POLICY_BENEFITS>> GetPPBEN_POLICY_BENEFITS(string policyNumber, string companyCode);

        Task<PPBEN_POLICY_BENEFITS?> GetPPBEN_POLICY_BENEFITS(
            string policyNumber,
            short benefitSeq,
            long benefitId,
            string companyCode);

        Task<PPBEN_POLICY_BENEFITS_TYPES_BA_OR?> GetPPBEN_POLICY_BENEFIT_TYPES_BA_OR(long pbenID);

        Task<PPBEN_POLICY_BENEFITS_TYPES_BF?> GetPPBEN_POLICY_BENEFIT_TYPES_BF(long pbenID);

        Task<PPBEN_POLICY_BENEFITS_TYPES_SU?> GetPPBEN_POLICY_BENEFIT_TYPES_SU(long pbenID);

        Task<PPBEN_POLICY_BENEFITS_TYPES_SL?> GetPPBEN_POLICY_BENEFIT_TYPES_SL(long pbenID);

        Task<PPBEN_POLICY_BENEFITS_TYPES_SP?> GetPPBEN_POLICY_BENEFIT_TYPES_SP(long pbenID);

        Task<List<PMUIN_MULTIPLE_INSUREDS>> GetPMUIN_MULTIPLE_INSUREDS(
            string policyNumber,
            int benefitSeq,
            int nameId,
            string relateCode,
            string companyCode);

        Task<ExtendedKeyQueryEntity> GetExtendedKey(string keyIdentifier, short iteration, short key);

        /// <summary>
        /// Look up extended key data for multiple benefits.
        /// </summary>
        /// <param name="lookup"></param>
        /// <returns></returns>
        List<ExtendedKeyLookupResult> GetExtendedKeyData(ExtendedKeysLookup lookup);

        /// <summary>
        /// Gets a list of AgentDTO records for the given company
        /// code and policy number, based on a join from PCOMC_COMMISSION_CONTROL_S to the PCOMC_COMMISSION_CONTROL table.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="policyNumber"></param>
        /// <returns></returns>
        Task<List<AgentDTO>> GetAgents(
            string companyCode,
            string policyNumber);

        /// <summary>
        /// Gets a list of policy relationship data by name id and address id.
        /// </summary>
        /// <param name="nameId"></param>
        /// <param name="addressId"></param>
        /// <returns></returns>
        Task<List<PolicyRelationship>> GetPolicyRelationships(int nameId, int addressId);

        /// <summary>
        /// Gets a list of policy requirement data by company code and policy number.
        /// </summary>
        /// <param name="companyCodeAndPolicyNumber"></param>
        /// <returns></returns>
        Task<List<PolicyRequirement>> GetPolicyRequirementsForHealth(
            CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber);

        /// <summary>
        /// Gets a list of policy requirement data related to Life by company code and policy number.
        /// </summary>
        /// <param name="companyCodeAndPolicyNumber"></param>
        /// <returns></returns>
        Task<List<PolicyRequirement>> GetPolicyRequirementsForLife(
            CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber);

        Task<Agent> GetAgentUpline(string agentNum, string marketCode, string agentLevel, DateTime issueDate);

        /// <summary>
        /// Gets the PolicyStatusDetail for the given policyNumber by querying
        /// the PPOLM_POLICY_BENEFIT_MISC and PICDA_WAIVER_DETAILS tables.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        Task<string?> GetPolicyStatusDetail(string policyNumber, string companyCode);

        /// <summary>
        /// Gets the EmployerDetail for the given policyNumber by querying
        /// the PGRUP_GROUP_MASTER  and PNAME tables.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        Task<EmployerDTO?> GetEmployerDetail(string policyNumber, string companyCode);

        Task<string?> GetUnderwritingClassDescription(string planCode, string underwritingClass);

        Task<string?> GetCoverageDescription(string planCode);

        bool IsInitialPaymentDeclined(string policyNumber, string companyCode);

        /// <summary>
        /// Gets the PPBEN_POLICY_BENEFIT_TYPES_BF record where PBEN_ID equals the passed in benefitId parameter.
        /// If found then it returns a bool based on whether the BF_DATE_NEGATIVE is greater than 0 or not.
        /// </summary>
        /// <param name="benefitId"></param>
        /// <returns></returns>
        Task<bool> IsULBenefitInGracePeriod(long benefitId);

        Task<PACON_ANNUITY_POLICY?> GetAnnuityPolicy(string policyNumber, string companyCode);

        HashSet<CompanyCodeAndPolicyNumber> GetMigratablePPOLCRecords();

        PolicyDTO GetPolicyDTO(string policyNumber, string companyCode);

        List<ParticipantDTO> GetParticipantDTOs(string policyNumber, string companyCode, List<string> relateCodes);

        List<ParticipantDTO> GetParticipantDTOsWithoutAddress(string policyNumber, string companyCode, List<string> relateCodes);

        List<BenefitDTO> GetBenefitDTOs(string policyNumber, string companyCode);

        List<PaymentDTO> GetPaymentData(string policyNumber, string companyCode);

        List<PolicyAgentDTO> GetPolicyAgentDTOs(string policyNumber, string companyCode);
    }
}