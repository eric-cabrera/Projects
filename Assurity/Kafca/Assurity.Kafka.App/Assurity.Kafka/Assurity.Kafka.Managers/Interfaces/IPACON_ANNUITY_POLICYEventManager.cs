namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for handling an updated PACON_ANNUITY_POLICY record.
    /// </summary>
    public interface IPACON_ANNUITY_POLICYEventManager
    {
        /// <summary>
        /// Process the event from the PACON_ANNUITY_POLICYEvent Topic. Updates
        /// all policies that are depending on this PACON_ANNUITY_POLICY. If policy
        /// does not yet exist this will create and save one to the mongodb.
        /// </summary>
        /// <param name="pacon">A PACON_ANNUITY_POLICY record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PACON_ANNUITY_POLICY pacon);
    }
}
