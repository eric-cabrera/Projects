namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// The class responsible for managing the handling an updated PNAME record.
    /// </summary>
    public interface IPNAMEEventManager
    {
        /// <summary>
        /// Process the event from the PNAMEEvent Topic. Updates
        /// all policies that are depending on this PNAME.
        /// </summary>
        /// <param name="pname">A PNAME record with updated data.</param>
        /// <param name="slowConsumer">Flag to treat event in slow consumer mode, which will bypass the
        /// KafkaSlowConsumerUpdateThreshold check.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PNAME pname, bool slowConsumer = false);
    }
}
