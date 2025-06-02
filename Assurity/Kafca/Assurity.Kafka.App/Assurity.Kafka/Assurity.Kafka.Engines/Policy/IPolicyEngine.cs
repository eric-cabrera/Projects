namespace Assurity.Kafka.Engines.Policy
{
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;

    /// <summary>
    /// The class responsible for managing data to create a Policy object or
    /// its parts.
    /// </summary>
    public interface IPolicyEngine
    {
        /// <summary>
        /// Builds and returns a list of Agents based on the LifePro commission control
        /// tables for a given policy.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <param name="applicationDate"></param>
        /// <returns></returns>
        Task<List<Agent>> GetAgents(string policyNumber, string companyCode, DateTime applicationDate);

        /// <summary>
        /// Builds and returns a list of Insureds for a given policy.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<Insured> GetInsureds(string policyNumber, string companyCode);

        /// <summary>
        /// Builds and returns a policy object for a given policyNumber.
        /// </summary>
        /// <param name="policyNumber">A Policy Number corresponding to a policy to be migrated.</param>
        /// <param name="companyCode">A policy's Company Code corresponding to a policy to be migrated.</param>
        /// <returns>The result of GetPolicy() and the policy object for a given policyNumber.</returns>
        Task<(GetPolicyResult, Policy?)> GetPolicy(string policyNumber, string companyCode);

        /// <summary>
        /// Returns PolicyStatusDetail.
        /// </summary>
        /// <param name="policyNumber">A Policy Number corresponding to a policy to be migrated.</param>
        /// <param name="companyCode">A policy's Company Code corresponding to a policy to be migrated.</param>
        /// <returns>A string value of Policy Status Detail.</returns>
        Task<PolicyStatusDetail> GetPolicyStatusDetail(string policyNumber, string companyCode);

        /// <summary>
        /// Updates requirement name for the list of given policies with the matching reqNumber.
        /// </summary>
        /// <param name="policies"></param>
        /// <param name="reqNumber"></param>
        /// <param name="reqDescription"></param>
        /// <returns></returns>
        List<Policy> UpdateRequirementName(
            List<Policy> policies,
            short reqNumber,
            string reqDescription);

        (ReturnPaymentType returnPaymentType, DateTime? returnPaymentDate) GetReturnPaymentData(string policyNumber, string companyCode);

        Task DeletePolicy(string policyNumber, string companyCode);

        Task<ProductDescription> GetBaseProductDescriptionByPlanCode(string planCode);
    }
}
