namespace Assurity.AgentPortal.Engines;

public interface IPdfUtilitiesEngine
{
    MemoryStream EnsurePortraitOrientation(MemoryStream memoryStream);
}
