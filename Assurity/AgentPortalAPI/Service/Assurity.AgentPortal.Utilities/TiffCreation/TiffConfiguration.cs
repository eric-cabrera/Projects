namespace Assurity.AgentPortal.Utilities.TiffCreation;

using SkiaSharp;

public class TiffConfiguration
{
    public float XAxisDpi { get; set; }

    public float YAxisDpi { get; set; }

    public SKColor TextColor { get; set; }

    public SKColor BackgroundColor { get; set; }

    public FontConfiguration? Font { get; set; }
}
