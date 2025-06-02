namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for managing the handling an
    /// updated PPBEN_POLICY_BENEFITS_TYPES_SU record.
    /// </summary>
    public interface IPPBEN_POLICY_BENEFITS_TYPES_SUEventManager
    {
        /// <summary>
        /// Process the event from the PPBEN_POLICY_BENEFITS_TYPES_SUEvent Topic.
        /// Updates all policies that are depending on this PPBEN_POLICY_BENEFITS_TYPES_SU.
        /// </summary>
        /// <param name="ppben">A PPBEN_POLICY_BENEFITS_TYPES_SU record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PPBEN_POLICY_BENEFITS_TYPES_SU ppben);
    }
}