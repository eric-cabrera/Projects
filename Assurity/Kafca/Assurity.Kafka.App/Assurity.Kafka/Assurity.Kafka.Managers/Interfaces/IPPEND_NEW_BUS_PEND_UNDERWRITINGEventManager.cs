namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for managing the handling an
    /// updated PPEND_NEW_BUS_PEND_UNDERWRITING record.
    /// </summary>
    public interface IPPEND_NEW_BUS_PEND_UNDERWRITINGEventManager
    {
        /// <summary>
        /// Process the event from the PPEND_NEW_BUS_PEND_UNDERWRITINGEvent Topic.
        /// Updates all policies that are depending on this PPEND_NEW_BUS_PEND_UNDERWRITING.
        /// </summary>
        /// <param name="pendu">A PPEND_NEW_BUS_PEND_UNDERWRITING record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PPEND_NEW_BUS_PEND_UNDERWRITING pendu);
    }
}