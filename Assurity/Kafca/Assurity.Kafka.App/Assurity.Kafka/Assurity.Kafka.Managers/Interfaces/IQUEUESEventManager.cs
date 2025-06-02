namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// Responsible for managing an updated AgentUseQueue record.
    /// </summary>
    public interface IQUEUESEventManager
    {
        /// <summary>
        /// Process the event from the QUEUESEvent Topic.
        /// Updates all policies that are depending on this AgentUseQueue.
        /// </summary>
        /// <param name="queues">A QUEUES record with updated data.</param>
        /// <param name="beforeQueue"></param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(QUEUES queues, string beforeQueue);
    }
}
