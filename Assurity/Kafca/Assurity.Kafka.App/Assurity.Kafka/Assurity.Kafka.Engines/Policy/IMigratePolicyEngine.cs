namespace Assurity.Kafka.Engines.Policy
{
    using Assurity.Kafka.Utilities.Enums;
    using Assurity.PolicyInfo.Contracts.V1;

    /// <summary>
    /// The class responsible for managing data to create a Policy object or
    /// its parts.
    /// </summary>
    public interface IMigratePolicyEngine
    {
        Task DeletePolicy(string policyNumber, string companyCode);

        Task<(GetPolicyResult, Policy)> GetPolicy(string policyNumber, string companyCode);
    }
}
