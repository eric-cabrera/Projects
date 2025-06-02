namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for managing the handling an
    /// updated PPEND_NEW_BUSINESS_PENDING record.
    /// </summary>
    public interface IPPEND_NEW_BUSINESS_PENDINGEventManager
    {
        /// <summary>
        /// Process the event from the PPEND_NEW_BUSINESS_PENDINGEvent Topic.
        /// Updates all policies that are depending on this PPEND_NEW_BUSINESS_PENDING.
        /// </summary>
        /// <param name="ppend">A PPEND_NEW_BUSINESS_PENDING record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PPEND_NEW_BUSINESS_PENDING ppend);
    }
}