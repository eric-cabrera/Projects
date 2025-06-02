namespace Assurity.AgentPortal.Accessors.Send;

using Assurity.AgentPortal.Accessors.DTOs;

public interface IGlobalDataAccessor
{
    Task<string> GetObjectIdForNewBusinessTransaction(string policyNumber);

    Task<List<AttributeObject>?> GetApplicationData(string policyNumber);
}