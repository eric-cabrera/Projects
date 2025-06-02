namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// Responsible for managing an updated PCOMC_COMMISSION_CONTROL_TYPE_S record.
    /// </summary>
    public interface IPCOMC_COMMISSION_CONTROL_TYPE_SEventManager
    {
        /// <summary>
        /// Process the event from the PCOMC_COMMISSION_CONTROL_TYPE_SEvent Topic.
        /// Updates all policies that are depending on this PCOMC_COMMISSION_CONTROL_TYPE_S.
        /// </summary>
        /// <param name="pcomc">A PCOMC_COMMISSION_CONTROL_TYPE_S record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PCOMC_COMMISSION_CONTROL_TYPE_S pcomc);
    }
}
