namespace Assurity.Kafka.Managers.Interfaces
{
    using Assurity.Kafka.Accessors.Entities;

    public interface ISysACAgentMarketCodesEventManager
    {
        Task ProcessEvent(SysACAgentMarketCodes sysACAgentMarketCode);
    }
}
