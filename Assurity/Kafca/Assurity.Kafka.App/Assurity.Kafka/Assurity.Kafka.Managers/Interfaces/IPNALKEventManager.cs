namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for managing the handling an updated PNALK record.
    /// </summary>
    public interface IPNALKEventManager
    {
        /// <summary>
        /// Process the event from the PNALKEvent Topic. Updates
        /// all policies that are depending on this PNALK.
        /// </summary>
        /// <param name="pnalk">A PNALK record with updated data.</param>
        /// <param name="slowConsumer">Flag to treat event in slow consumer mode, which will bypass the
        /// KafkaSlowConsumerUpdateThreshold check.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PNALK pnalk, bool slowConsumer = false);
    }
}
