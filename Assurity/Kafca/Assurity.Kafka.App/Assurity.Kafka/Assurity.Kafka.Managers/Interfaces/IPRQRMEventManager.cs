namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// Responsible for managing an updated PRQRM record.
    /// </summary>
    public interface IPRQRMEventManager
    {
        /// <summary>
        /// Process the event from the PRQRMEvent Topic.
        /// Updates all policies that are dependent on this PRQRM record.
        /// </summary>
        /// <param name="prqrm">A PRQRM record with updated data.</param>
        /// <returns></returns>
        Task ProcessEvent(PRQRM prqrm);
    }
}