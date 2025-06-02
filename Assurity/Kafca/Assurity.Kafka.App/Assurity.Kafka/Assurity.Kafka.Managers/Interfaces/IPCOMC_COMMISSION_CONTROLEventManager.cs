namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// Responsible for managing an updated PCOMC_COMMISSION_CONTROL record.
    /// </summary>
    public interface IPCOMC_COMMISSION_CONTROLEventManager
    {
        /// <summary>
        /// Process the event from the PCOMC_COMMISSION_CONTROLEvent Topic.
        /// Updates all policies that are depending on this PCOMC_COMMISSION_CONTROL..
        /// </summary>
        /// <param name="pcomc">A PCOMC_COMMISSION_CONTROL record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PCOMC_COMMISSION_CONTROL pcomc);
    }
}
