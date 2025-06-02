namespace Assurity.AgentPortal.Contracts.Send;

using Assurity.AgentPortal.Contracts.Send.Enums;

public class File
{
    public byte[] Bytes { get; set; }

    public FileType FileType { get; set; }

    public string Name { get; set; }
}