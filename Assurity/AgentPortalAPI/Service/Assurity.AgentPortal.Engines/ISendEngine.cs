namespace Assurity.AgentPortal.Engines;

using Assurity.AgentPortal.Contracts.Send;
using DTOs = Assurity.AgentPortal.Accessors.DTOs;

public interface ISendEngine
{
    Task<List<DTOs.File>> CreateImageFiles(List<File> files);

    Task<DTOs.File> CreateMessageFile(string message);

    Task<DTOs.File> CreateJsonMessageFile(string messageJson);
}
