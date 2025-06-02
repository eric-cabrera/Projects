namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for managing the handling an updated PADDR record.
    /// </summary>
    public interface IPADDREventManager
    {
        /// <summary>
        /// Process the event from the PADDREvent Topic. Updates
        /// all policies that are depending on this PADDR.
        /// </summary>
        /// <param name="paddr">A PADDR record with updated data.</param>
        /// <param name="slowConsumer">Flag to treat event in slow consumer mode, which will bypass the
        /// KafkaSlowConsumerUpdateThreshold check.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PADDR paddr, bool slowConsumer = false);
    }
}
