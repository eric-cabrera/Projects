namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for managing the handling an
    /// updated IPPBEN_POLICY_BENEFITS_TYPES_BF record.
    /// </summary>
    public interface IPPBEN_POLICY_BENEFITS_TYPES_BFEventManager
    {
        /// <summary>
        /// Process the event from the IPPBEN_POLICY_BENEFITS_TYPES_BFEvent Topic.
        /// Updates all policies that are depending on this IPPBEN_POLICY_BENEFITS_TYPES_BF.
        /// </summary>
        /// <param name="ppben">A IPPBEN_POLICY_BENEFITS_TYPES_BF record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PPBEN_POLICY_BENEFITS_TYPES_BF ppben);
    }
}