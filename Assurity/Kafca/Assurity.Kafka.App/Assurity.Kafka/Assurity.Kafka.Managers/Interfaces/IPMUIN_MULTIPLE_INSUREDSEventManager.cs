namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for managing the handling an
    /// updated PMUIN_MULTIPLE_INSUREDS record.
    /// </summary>
    public interface IPMUIN_MULTIPLE_INSUREDSEventManager
    {
        /// <summary>
        /// Process the event from the PMUIN_MULTIPLE_INSUREDSEvent Topic.
        /// Updates all policies that are depending on this PMUIN_MULTIPLE_INSUREDS.
        /// </summary>
        /// <param name="pmuin">A PMUIN_MULTIPLE_INSUREDS record with updated data.</param>
        /// <param name="changeType">Change Type of the event.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PMUIN_MULTIPLE_INSUREDS pmuin, string changeType);
    }
}