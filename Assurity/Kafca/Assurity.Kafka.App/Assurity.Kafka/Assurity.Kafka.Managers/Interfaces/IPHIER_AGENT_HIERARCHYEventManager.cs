namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    /// <summary>
    /// Responsible for managing an updated PHIER_AGENT_HIERARCHY record.
    /// </summary>
    public interface IPHIER_AGENT_HIERARCHYEventManager
    {
        /// <summary>
        /// Process the event from the PHIER_AGENT_HIERARCHY Event Topic.
        /// Updates all policies that are depending on this PHIER_AGENT_HIERARCHY.
        /// </summary>
        /// <param name="phier">A PHIER_AGENT_HIERARCHY record with updated data.</param>
        /// <param name="changeType">Change Type of the event.</param>
        /// <param name="beforeAgentNum">Before Agent Num of the deleted agent.</param>
        /// <param name="slowConsumer">Flag to treat event in slow consumer mode, which will bypass the
        /// KafkaSlowConsumerUpdateThreshold check.</param>
        /// <returns><see cref="Task{TResult}"/>.</returns>
        Task ProcessEvent(PHIER_AGENT_HIERARCHY phier, string changeType, string beforeAgentNum, bool slowConsumer = false);
    }
}
