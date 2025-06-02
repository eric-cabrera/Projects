namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// Responsible for managing an updated SysNBRequirements record.
    /// </summary>
    public interface ISysNBRequirementsEventManager
    {
        /// <summary>
        /// Process the event from the SysNBRequirementsEvent Topic.
        /// Updates all policies that are depending on this SysNBRequirements.
        /// </summary>
        /// <param name="sysNBRequirements">A SysNBRequirements record with updated data.</param>
        /// <returns></returns>
        Task ProcessEvent(SysNBRequirements sysNBRequirements);
    }
}