namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for managing the handling an updated PPOLC record.
    /// </summary>
    public interface IPPOLCEventManager
    {
        /// <summary>
        /// Process the event from the PPOLCEvent Topic. Updates
        /// all policies that are depending on this PPOLC. If policy
        /// does not yet exist this will create and save one to the mongodb.
        /// </summary>
        /// <param name="ppolc">A PPOLC record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PPOLC ppolc);
    }
}
