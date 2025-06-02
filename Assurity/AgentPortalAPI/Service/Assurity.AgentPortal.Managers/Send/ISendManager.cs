namespace Assurity.AgentPortal.Managers.Send;

using Assurity.AgentPortal.Contracts.Send;

public interface ISendManager
{
    Task SendMessageAndFilesToGlobal(ActionRequest actionRequest);
}