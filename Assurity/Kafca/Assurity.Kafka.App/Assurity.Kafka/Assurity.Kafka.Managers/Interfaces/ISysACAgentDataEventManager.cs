namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    public interface ISysACAgentDataEventManager
    {
        Task ProcessEvent(SysACAgentData sysACAgentData);
    }
}
