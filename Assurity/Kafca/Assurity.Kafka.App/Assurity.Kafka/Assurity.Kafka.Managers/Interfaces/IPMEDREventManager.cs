namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// Responsible for managing an updated PMEDR record.
    /// </summary>
    public interface IPMEDREventManager
    {
        /// <summary>
        /// Process the event from the PMEDREvent Topic.
        /// Updates all policies that are dependent on this PMEDR record.
        /// </summary>
        /// <param name="pmedr">A PMEDR record with updated data.</param>
        /// <returns></returns>
        Task ProcessEvent(PMEDR pmedr);
    }
}