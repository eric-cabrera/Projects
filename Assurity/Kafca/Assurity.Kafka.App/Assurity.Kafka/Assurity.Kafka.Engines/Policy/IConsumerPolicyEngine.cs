namespace Assurity.Kafka.Engines.Policy
{
    using Assurity.Kafka.Accessors.DataTransferObjects;
    using Assurity.Kafka.Accessors.Entities;
    using Assurity.PolicyInfo.Contracts.V1;
    using Assurity.PolicyInfo.Contracts.V1.Enums;

    /// <summary>
    /// The class responsible for managing data to create a Policy object or
    /// its parts.
    /// </summary>
    public interface IConsumerPolicyEngine : IPolicyEngine
    {
        Benefit GetBenefit(CompanyCodeAndPolicyNumber companyCodeAndPolicyNumber, LineOfBusiness lineOfBusiness, PPBEN_POLICY_BENEFITS ppben);

        /// <summary>
        /// Builds and returns a list of Annuitants for a given policy.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<Annuitant> GetAnnuitants(string policyNumber, string companyCode);

        /// <summary>
        /// Builds and returns an Assignee for a given policy.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        Assignee GetAssignee(string policyNumber, string companyCode);

        /// <summary>
        /// Builds and returns a list of Beneficiaries for a given policy.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<Beneficiary> GetBeneficiaries(string policyNumber, string companyCode);

        /// <summary>
        /// Builds and returns a payee for a given policy.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        Payee GetPayee(string policyNumber, string companyCode);

        /// <summary>
        /// Builds and returns a list of Owners for a given policy.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<Owner> GetOwners(string policyNumber, string companyCode);

        /// <summary>
        /// Builds and returns a list of Payors for a given policy.
        /// </summary>
        /// <param name="policyNumber"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<Payor> GetPayors(string policyNumber, string companyCode);

        Task<List<Requirement>> GetRequirements(Policy policy);

        Task<Employer> GetEmployer(string policyNumber, string companyCode);
    }
}
