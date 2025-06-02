namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// Responsible for managing an updated PBDRV record.
    /// </summary>
    public interface IPBDRVEventManager
    {
        /// <summary>
        /// Process the event from the PBDRVEvent Topic.
        /// Updates all policies that are dependent on this PBDRV record.
        /// </summary>
        /// <param name="pbdrv">A PBDRV record with updated data.</param>
        /// <returns></returns>
        Task ProcessEvent(PBDRV pbdrv);
    }
}