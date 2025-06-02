namespace Assurity.AgentPortal.Managers.PolicyInfo;

using Assurity.AgentPortal.Contracts.Shared;

public interface IExecute360DocumentManager
{
    Task<FileResponse?> GetApplication(string policyNumber);
}