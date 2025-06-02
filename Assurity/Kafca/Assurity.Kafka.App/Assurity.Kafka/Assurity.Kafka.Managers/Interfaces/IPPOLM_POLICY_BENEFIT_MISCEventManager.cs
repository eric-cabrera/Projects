namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for managing the handling an updated PPOLM_POLICY_BENEFITS_MISC record.
    /// </summary>
    public interface IPPOLM_POLICY_BENEFIT_MISCEventManager
    {
        /// <summary>
        /// Process the event from the PPOLM_POLICY_BENEFIT_MISC Topic. Updates
        /// all policies that are depending on this PPOLM_POLICY_BENEFIT_MISC.
        /// </summary>
        /// <param name="ppolm">A PPOLM_POLICY_BENEFIT_MISC record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PPOLM_POLICY_BENEFIT_MISC ppolm);
    }
}
