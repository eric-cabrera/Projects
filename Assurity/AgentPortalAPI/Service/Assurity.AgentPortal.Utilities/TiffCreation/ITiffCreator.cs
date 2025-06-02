namespace Assurity.AgentPortal.Utilities.TiffCreation;

public interface ITiffCreator
{
    Task<string> CreateMultiPage(string[] text);

    Task<string> CreateTiffWithCustomStyling(string messageJson);
}