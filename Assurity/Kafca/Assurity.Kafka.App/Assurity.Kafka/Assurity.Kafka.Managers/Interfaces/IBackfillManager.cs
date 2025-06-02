namespace Assurity.Kafka.Managers.Interfaces
{
    using System.Threading.Tasks;
    using Assurity.Kafka.Accessors.DataTransferObjects;

    /// <summary>
    /// The class responsible for managing the logic of creating a policy.
    /// </summary>
    public interface IBackfillManager
    {
        Task BackFillPolicies(string stringExecutionMode);

        /// <summary>
        /// Used for debugging.
        /// </summary>
        /// <param name="lifeProPolicy"></param>
        /// <returns></returns>
        Task MigrateSinglePolicy(CompanyCodeAndPolicyNumber lifeProPolicy);

        void FlagPastDuePolicies();

        void FlagPendingPolicies();
    }
}