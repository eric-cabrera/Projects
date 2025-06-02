namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// Responsible for managing an updated PRQRMTBL record.
    /// </summary>
    public interface IPRQRMTBLEventManager
    {
        /// <summary>
        /// Process the event from the PRQRMTBLEvent Topic.
        /// Updates all policies that are dependent on this PRQRMTBL record.
        /// </summary>
        /// <param name="prqrmtbl">A PRQRMTBL record with updated data.</param>
        /// <returns></returns>
        Task ProcessEvent(PRQRMTBL prqrmtbl);
    }
}