namespace Assurity.Kafka.Accessors
{
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.PolicyInfo.Contracts.V1;
    using MongoDB.Driver;

    public interface IEventsAccessor
    {
        /// <summary>
        /// Retrieves all the documents from the RequirementMapping collection.
        /// </summary>
        /// <returns>Returns a list of RequirementMapping.</returns>
        Task<List<RequirementMapping>> GetAllRequirementMappingsAsync();

        List<RequirementMapping> GetRequirementMappings(List<int> ids);

        /// <summary>
        /// Finds the RequirementMapping object by requirementId.
        /// </summary>
        /// <param name="requirementId"></param>
        /// <returns>The RequirementMapping.</returns>
        Task<RequirementMapping?> GetRequirementMappingAsync(int requirementId);

        /// <summary>
        /// Saves an policy to the cache.
        /// </summary>
        /// <param name="policy">Policy to be stored to collection.</param>
        /// <returns>A cacheId that can be used to look up the case on subsequent calls.</returns>
        Task<string> CreatePolicyAsync(Policy policy);

        Task CreateOrReplacePolicyAsync(Policy policy);

        HashSet<CompanyCodeAndPolicyNumber> GetAllCompanyCodesAndPolicyNumbers();

        /// <summary>
        /// Finds by policy number and returns the policy object.
        /// </summary>
        /// <param name="policyNumber">Number of Policy to be found.</param>
        /// <returns>The policy.</returns>
        Task<Policy?> GetPolicyAsync(string policyNumber);

        /// <summary>
        /// Finds by policy number and company code and returns the policy object.
        /// </summary>
        /// <param name="policyNumber">Number of Policy to be found.</param>
        /// <param name="companyCode">Company code of the Policy to be found.</param>
        /// <returns>The policy.</returns>
        Task<Policy?> GetPolicyAsync(string policyNumber, string companyCode);

        /// <summary>
        /// Finds all policies with the given requirement number matching in one of their requirements.
        /// </summary>
        /// <param name="reqNumber"></param>
        /// <returns></returns>
        Task<List<Policy>> GetPoliciesAsync(short reqNumber);

        /// <summary>
        /// Finds all polices with the given Employer Group Number matching in their Employer object.
        /// </summary>
        /// <param name="groupNumber"><see cref="Employer.Number"/>.</param>
        /// <returns></returns>
        Task<List<Policy>> GetPoliciesByGroupNumber(string groupNumber);

        List<CompanyCodeAndPolicyNumber> GetCompanyCodeAndPolicyNumberOfFlaggedPolicies();

        long FlagPendingPolicies();

        long FlagPastDuePolicies();

        Task<List<Policy>> GetInitialPaymentDeclinedPoliciesWithPassedRetentionDurationAsync(DateTime batchStartDate);

        IEnumerable<string> GetPolicyNumbersForDeletion(DateTime batchStartDate);

        /// <summary>
        /// Checks of a policy is already in the collection.
        /// </summary>
        /// <param name="policyNumber">Policy number to be checked.</param>
        /// /// <param name="companyCode">Company code of policy to be checked.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<bool> CheckIfPolicyExists(string policyNumber, string companyCode);

        Task<long> UpdatePolicyAsync<T>(Policy policyIn, T obj, string objType);

        Task<long> UpdatePolicyAsync<T>(Policy policyIn, Dictionary<string, T> objDictionary);

        Task<long> UpdatePolicyBenefitsAsync<T>(Policy policyIn, Dictionary<string, T> objDictionary, long benefitId);

        Task<long> UpdateNameAndEmailAddressInPolicyRequirements(Policy policyIn, Person newPerson, bool isBusiness);

        Task<long> UpdateAddressInPolicyRequirements(Policy policyIn, Address newAddress);

        Task<long> UpdatePhoneNumberInPolicyRequirements(Policy policyIn, int nameId, string phoneNumber);

        Task UpdateOrCreatePolicyHierarchyAsync(PolicyHierarchy policyHierarchy);

        Task<string> UpdatePolicyHierarchyAsync(string policyNumber, string companyCode, List<AgentHierarchy> agentHierarchies);

        Task<long> UpdatePastDuePoliciesAsync(List<string> policyNumbers);

        /// <summary>
        /// Finds by policy number and returns the policy hierarchy object.
        /// </summary>
        /// <param name="policyNumber">Number of Policy to be found.</param>
        /// <param name="companyCode">Company code of the Policy to be found.</param>
        /// <returns>The policy hierarchy.</returns>
        Task<PolicyHierarchy?> GetPolicyHierarchyAsync(string policyNumber, string companyCode);

        /// <summary>
        /// Finds by AgentId, CompanyCode and returns the AgentPolicyAccess object.
        /// </summary>
        /// <param name="agentId">AgentId.</param>
        /// <param name="companyCode">Company code of the Policy to be found.</param>
        /// <returns>The agent policy access.</returns>
        Task<AgentPolicyAccess?> GetAgentPolicyAccessAsync(string agentId, string companyCode);

        /// <summary>
        /// Adds a new policy number to an existing AgentPolicyAccess document. If none exists, creates a new document.
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        Task<long> InsertAgentPolicyAccessAsync(string agentId, string policyNumber, string companyCode);

        /// <summary>
        /// Removes an AgentId from AgentPolicyAccess based on given policy number and company code.
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        Task<long> RemoveAgentPolicyAccessAsync(string agentId, string policyNumber, string companyCode);

        Task<string> InsertPolicyBenefitAsync(Benefit benefit, string policyNumber, string companyCode);

        /// <summary>
        /// Removes a Benefit Policies based on given policy number, company code, and benefit ID.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <param name="benefitId"></param>
        /// <returns></returns>
        Task<string> RemovePolicyBenefitByBenefitIdAsync(string policyNumber, string companyCode, long benefitId);

        Task<long> UpdateAgentPolicyAccessListAsync(IClientSessionHandle session, List<string> removePolicyNumbers);

        /// <summary>
        /// Deletes a policy object.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        Task<long> DeletePolicyAsync(string policyNumber, string companyCode);

        Task<long> DeletePolicyHierarchyAsync(string policyNumber, string companyCode);

        Task DeletePoliciesAsync(IEnumerable<string> policyNumbers);

        Task<long> DeletePoliciesAsync(IClientSessionHandle session, List<string> policyNumbers);

        void DeletePolicyHierarchies(List<CompanyCodeAndPolicyNumber> companyCodesAndPolicyNumbers);

        Task<long> DeletePolicyHierarchiesAsync(IClientSessionHandle session, List<string> policyNumbers);

        Task<List<Policy>> GetPoliciesWithAgentsByAddressIdAsync(int addrId);

        Task<List<Policy>> GetPoliciesWithAgentsByNameIdAsync(int nameId);

        Task<List<Policy>> GetPoliciesWithAnnuitantsByAddressIdAsync(int addrId);

        Task<List<Policy>> GetPoliciesWithAnnuitantsByNameIdAsync(int nameId);

        Task<List<Policy>> GetPoliciesWithAssigneeByAddressIdAsync(int addrId);

        Task<List<Policy>> GetPoliciesWithAssigneeByNameIdAsync(int nameId);

        Task<List<Policy>> GetPoliciesWithBeneficiariesByAddressIdAsync(int addrId);

        Task<List<Policy>> GetPoliciesWithBeneficiariesByNameIdAsync(int nameId);

        Task<List<Policy>> GetPoliciesWithEmployerByNameIdAsync(int nameId);

        Task<List<Policy>> GetPoliciesWithInsuredsByAddressIdAsync(int addrId);

        Task<List<Policy>> GetPoliciesWithInsuredsByNameIdAsync(int nameId);

        Task<List<Policy>> GetPoliciesWithOwnersByAddressIdAsync(int addrId);

        Task<List<Policy>> GetPoliciesWithOwnersByNameIdAsync(int nameId);

        Task<List<Policy>> GetPoliciesWithPayorsByAddressIdAsync(int addrId);

        Task<List<Policy>> GetPoliciesWithPayorsByNameIdAsync(int nameId);

        Task<List<Policy>> GetPoliciesWithPayeeByAddressIdAsync(int addrId);

        Task<List<Policy>> GetPoliciesWithPayeeByNameIdAsync(int nameId);

        Task<List<Policy>> GetPoliciesWithRequirementsByAddressIdAsync(int addrId);

        Task<List<Policy>> GetPoliciesWithRequirementsByNameIdAsync(int nameId);
    }
}
