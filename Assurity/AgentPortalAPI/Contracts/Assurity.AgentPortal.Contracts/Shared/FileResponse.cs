namespace Assurity.AgentPortal.Contracts.Shared;

public class FileResponse
{
    public FileResponse(string fileName, string fileType)
    {
        FileName = fileName;
        FileType = fileType;
        FileData = Array.Empty<byte>();
    }

    public string FileName { get; set; }

    public string FileType { get; set; }

    public byte[] FileData { get; set; }

    public Error? Error { get; set; }
}
