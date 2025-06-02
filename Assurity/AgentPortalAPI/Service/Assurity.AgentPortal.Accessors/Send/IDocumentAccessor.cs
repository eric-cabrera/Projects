namespace Assurity.AgentPortal.Accessors.Send;

using Assurity.AgentPortal.Accessors.DTOs;

public interface IDocumentAccessor
{
    void SendDocuments(ActionRequest actionRequest);
}