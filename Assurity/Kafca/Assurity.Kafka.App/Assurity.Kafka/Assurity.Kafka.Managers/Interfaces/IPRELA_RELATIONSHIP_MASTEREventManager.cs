namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for managing the handling an updated PRELA_RELATIONSHIP_MASTER record.
    /// </summary>
    public interface IPRELA_RELATIONSHIP_MASTEREventManager
    {
        /// <summary>
        /// Process the event from the PRELA_RELATIONSHIP_MASTEREvent Topic. Updates
        /// all policies that are depending on this PADDR.
        /// </summary>
        /// <param name="prela">A PRELA_RELATIONSHIP_MASTER record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PRELA_RELATIONSHIP_MASTER prela);
    }
}
