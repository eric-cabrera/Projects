namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for handling an updated PACTG record.
    /// </summary>
    public interface IPACTGEventManager
    {
        /// <summary>
        /// Process the event from the PACTGEvent Topic. Updates all policies that are depending on this PACTG.
        /// </summary>
        /// <param name="pactg">A PACTG record with updated data.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PACTG pactg);
    }
}
