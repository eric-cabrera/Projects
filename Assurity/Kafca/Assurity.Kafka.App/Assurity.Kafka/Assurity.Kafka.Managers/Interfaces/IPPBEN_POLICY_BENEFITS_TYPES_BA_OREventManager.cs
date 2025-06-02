namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for managing the handling an
    /// updated PPBEN_POLICY_BENEFITS_TYPES_BA_OR record.
    /// </summary>
    public interface IPPBEN_POLICY_BENEFITS_TYPES_BA_OREventManager
    {
        /// <summary>
        /// Process the event from the PPBEN_POLICY_BENEFITS_TYPES_BA_OREvent Topic.
        /// Updates all policies that are depending on this PPBEN_POLICY_BENEFITS_TYPES_BA_OR.
        /// </summary>
        /// <param name="ppben">A PPBEN_POLICY_BENEFITS_TYPES_BA_OR record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PPBEN_POLICY_BENEFITS_TYPES_BA_OR ppben);
    }
}