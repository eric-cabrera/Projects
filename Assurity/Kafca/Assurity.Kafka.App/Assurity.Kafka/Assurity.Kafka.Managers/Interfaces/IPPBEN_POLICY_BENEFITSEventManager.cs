namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for managing the handling an
    /// updated PPBEN_POLICY_BENEFITS record.
    /// </summary>
    public interface IPPBEN_POLICY_BENEFITSEventManager
    {
        /// <summary>
        /// Process the event from the PPBEN_POLICY_BENEFITSEvent Topic.
        /// Updates all policies that are depending on this PPBEN_POLICY_BENEFITS.
        /// </summary>
        /// <param name="ppben">A PPBEN_POLICY_BENEFITS record with updated data.</param>
        /// <param name="changeType">Change Type of the event.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PPBEN_POLICY_BENEFITS ppben, string changeType);
    }
}