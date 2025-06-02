namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    public interface ISysZ9ProcessEventManager
    {
        Task ProcessEvent(SysZ9Process sysZ9Process);
    }
}
